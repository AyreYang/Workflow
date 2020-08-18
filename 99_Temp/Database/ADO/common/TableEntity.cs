using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.common.messages;
using DataBase.common.objects;
using Database.ADO.interfaces;

namespace DataBase.common
{
    public abstract class TableEntity : IDisposable
    {
        //private DatabaseCore accessor { get; set; }
        protected DatabaseCore accessor { get; private set; }
        public string TableName { get; private set; }
        #region public members
        public long FreshTimeTicks { get; private set; }
        internal EntityState EntityStatus { get; private set; }
        public string SQLTableSelect
        {
            get
            {
                var pattern = "SELECT {0} FROM [{1}]";
                var columns = new List<string>();
                (new List<string>(Columns.Keys)).ForEach(col => columns.Add(col));
                var items = string.Join(",", columns);
                return string.Format(pattern, items, TableName);
            }
        }
        public string SQLTableTruncate
        {
            get
            {
                return string.Format("TRUNCATE TABLE [{0}]", TableName);
            }
        }
        #endregion

        public void Copy(TableEntity entity, params string[] excludes)
        {
            if (entity == null) return;
            foreach (string key in Columns.Keys)
            {
                if (excludes != null && excludes.Any(col => col.Equals(key))) continue;
                if (entity.Columns.ContainsKey(key) && Columns[key].KeyType == KeyType.Normal)
                {
                    var column = entity.GetColumn(key);
                    if (column.DataType.Equals(Columns[key].DataType))
                    {
                        this.SetValue(key, column.Value);
                    }
                }
            }
        }

        private Dictionary<string, DBColumn> Columns { get; set; }
        public int ColumnCount { get { return Columns.Count; } }
        internal List<DBColumn> PrimaryList
        {
            get
            {
                var columns = new List<DBColumn>(Columns.Values);
                return columns.FindAll(col => col.KeyType == KeyType.Primary || col.KeyType == KeyType.IncrementPrimary);
            }
        }
        internal string PrimaryKeys
        {
            get
            {
                var keys = new StringBuilder();
                PrimaryList.ForEach(col =>
                {
                    keys.Append(string.Format("[{0}:'{1}']", col.ID, (col.Value != null) ? col.Value.ToString() : "null"));
                });
                return keys.ToString();
            }
        }
        internal Dictionary<string, DBForeign> Foreigns { get; set; }

        public TableEntity(string table) : this(table, null) { }
        public TableEntity(string table, DatabaseCore accessor)
        {
            if (string.IsNullOrWhiteSpace(table)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "table"));
            //if (accessor == null) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL, "accessor"));

            this.TableName = table.Trim();
            this.accessor = accessor;

            if (this.accessor != null && !this.accessor.TableExists(this.TableName)) throw new Exception(string.Format(GeneralMessages.ERR_TABLE_DOES_NOT_EXIST, this.TableName));

            FreshTimeTicks = 0;
            Columns = new Dictionary<string, DBColumn>();
            Foreigns = new Dictionary<string, DBForeign>();

            InitialColumns();
            InitialForeigns();

            SetEntityState();
        }

        public static T Create<T>() where T : TableEntity, new()
        {
            T entity = default(T);
            try
            {
                entity = typeof(T).Assembly.CreateInstance(typeof(T).ToString()) as T;
            }
            catch { }
            return entity;
        }


        #region Internal Methods
        internal void SetDBAccessor(DatabaseCore accessor)
        {
            if (accessor != null) this.accessor = accessor;
        }
        internal int SetEntity(DataRow row, long time = 0)
        {
            if (row == null) return 0;

            foreach (DBColumn column in Columns.Values) column.Fresh(row);
            foreach (string key in Foreigns.Keys) Foreigns[key].Fresh(accessor);
            SetEntityState();
            FreshTimeTicks = time;
            return 1;
        }
        protected virtual void SetFromEntity(TableEntity entity)
        {
            foreach (var column1 in entity.Columns.Values)
            {
                var column2 = this.FindColumn(column1.ID);
                if (column2 == null) continue;
                this.SetColumn(column2.ID, column1.Value);
            }
        }
        internal int SetEntity(TableEntity entity)
        {
            if (entity == null) return 0;
            SetFromEntity(entity);
            SetEntityState();
            FreshTimeTicks = DateTime.Now.Ticks;
            return 1;
        }
        internal DbCommand GetInsertCommand(DatabaseCore accessor = null)
        {
            DbCommand command = null;
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return null;
            string text = null;
            List<DbParameter> list = null;
            InsertSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text))
            {
                command = dba.CreateCommand(text, list);
            }
            return command;
        }
        internal List<DbCommand> INTERNAL_GetInsertCommands(DatabaseCore accessor = null)
        {
            return GetInsertCommands(accessor);
        }
        protected virtual List<DbCommand> GetInsertCommands(DatabaseCore accessor = null)
        {
            var commands = new List<DbCommand>();

            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return commands;
            if (EntityStatus != EntityState.ASSIGNED) return commands;
            string text = null;
            List<DbParameter> list = null;
            InsertSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text))
            {
                commands.AddRange(GetDeleteCommands());
                commands.Add(dba.CreateCommand(text, list));
            }

            return commands;
        }
        internal List<IADbCommand> INTERNAL_GetInsertADbCommands(DatabaseCore accessor = null)
        {
            return GetInsertADbCommands(accessor);
        }

        protected virtual List<IADbCommand> GetInsertADbCommands(DatabaseCore accessor = null)
        {
            var commands = new List<IADbCommand>();

            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return commands;
            if (EntityStatus != EntityState.ASSIGNED) return commands;
            string text = null;
            List<DbParameter> list = null;
            InsertSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text))
            {
                commands.AddRange(GetDeleteADbCommands());
                commands.Add(new ADbCommand(dba.CreateCommand(text, list), CallbackAction_INSERT));
            }

            return commands;
        }
        protected virtual void CallbackAction_INSERT(DbCommand command, List<DbCommand> commands)
        {
        }
        internal List<DbCommand> INTERNAL_GetUpdateCommands(DatabaseCore accessor = null)
        {
            return GetUpdateCommands(accessor);
        }
        protected virtual List<DbCommand> GetUpdateCommands(DatabaseCore accessor = null)
        {
            var commands = new List<DbCommand>();
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return commands;
            if (EntityStatus != EntityState.CHANGED) return commands;
            string text = null;
            List<DbParameter> list = null;
            UpdateSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text)) commands.Add(dba.CreateCommand(text, list));

            return commands;
        }
        internal List<IADbCommand> INTERNAL_GetUpdateADbCommands(DatabaseCore accessor = null)
        {
            return GetUpdateADbCommands(accessor);
        }
        protected virtual List<IADbCommand> GetUpdateADbCommands(DatabaseCore accessor = null)
        {
            var commands = new List<IADbCommand>();
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return commands;
            if (EntityStatus != EntityState.CHANGED) return commands;
            string text = null;
            List<DbParameter> list = null;
            UpdateSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text)) commands.Add(new ADbCommand(dba.CreateCommand(text, list), CallbackAction_UPDATE));

            return commands;
        }

        internal DbCommand GetUpdateCommand(DatabaseCore accessor = null)
        {
            DbCommand command = null;
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return null;
            string text = null;
            List<DbParameter> list = null;
            UpdateSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text))
            {
                command = dba.CreateCommand(text, list);
            }
            return command;
        }
        protected virtual void CallbackAction_UPDATE(DbCommand command, List<DbCommand> commands)
        {
        }
        internal List<DbCommand> INTERNAL_GetSaveCommands(DatabaseCore accessor = null)
        {
            return GetSaveCommands(accessor);
        }
        protected virtual List<DbCommand> GetSaveCommands(DatabaseCore accessor = null)
        {
            var commands = new List<DbCommand>();
            if (this.EntityStatus > EntityState.RAW)
            {
                var dba = (accessor != null) ? accessor : this.accessor;
                if (dba == null) return commands;

                //// Save Foreigns
                //(new List<DBForeign>(Foreigns.Values)).ForEach(foreign =>
                //{
                //    commands.AddRange(foreign.GetSaveCommands(dba));
                //});

                // Save Self
                List<DbCommand> list = null;
                if (this.EntityStatus == EntityState.ASSIGNED) list = GetInsertCommands(dba);
                if (this.EntityStatus == EntityState.CHANGED) list = GetUpdateCommands(dba);
                if (list != null && list.Count > 0) commands.AddRange(list);

                // Save Foreigns
                (new List<DBForeign>(Foreigns.Values)).ForEach(foreign =>
                {
                    commands.AddRange(foreign.GetSaveCommands(dba));
                });
            }
            return commands;
        }
        internal List<IADbCommand> INTERNAL_GetSaveADbCommands(DatabaseCore accessor = null)
        {
            return GetSaveADbCommands(accessor);
        }

        protected virtual List<IADbCommand> GetSaveADbCommands(DatabaseCore accessor = null)
        {
            var commands = new List<IADbCommand>();
            if (this.EntityStatus > EntityState.RAW)
            {
                var dba = (accessor != null) ? accessor : this.accessor;
                if (dba == null) return commands;

                //// Save Foreigns
                //(new List<DBForeign>(Foreigns.Values)).ForEach(foreign =>
                //{
                //    commands.AddRange(foreign.GetSaveCommands(dba));
                //});

                // Save Self
                List<DbCommand> list = null;
                Action<DbCommand, List<DbCommand>> action = null;
                if (this.EntityStatus == EntityState.ASSIGNED)
                {
                    list = GetInsertCommands(dba);
                    action = CallbackAction_INSERT;
                }
                if (this.EntityStatus == EntityState.CHANGED)
                {
                    list = GetUpdateCommands(dba);
                    action = CallbackAction_UPDATE;
                }
                if (list != null && list.Count > 0)foreach(var item in list)
                {
                    commands.Add(new ADbCommand(item, action));
                }

                // Save Foreigns
                (new List<DBForeign>(Foreigns.Values)).ForEach(foreign =>
                {
                    commands.AddRange(foreign.GetSaveADbCommands(dba));
                });
            }
            return commands;
        }

        internal DbCommand GetDeleteCommand(DatabaseCore accessor = null)
        {
            DbCommand command = null;
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return null;
            string text = null;
            List<DbParameter> list = null;
            DeleteSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text))
            {
                command = dba.CreateCommand(text, list);
            }
            return command;
        }
        internal List<DbCommand> INTERNAL_GetDeleteCommands(DatabaseCore accessor = null)
        {
            return GetDeleteCommands(accessor);
        }
        protected virtual List<DbCommand> GetDeleteCommands(DatabaseCore accessor = null)
        {
            var commands = new List<DbCommand>();
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return commands;
            if (EntityStatus < EntityState.FRESHED) return commands;

            // Delete Foreigns
            (new List<DBForeign>(Foreigns.Values)).ForEach(foreign =>
            {
                commands.AddRange(foreign.GetDeleteCommands(dba));
            });

            // Delete Self
            string text = null;
            List<DbParameter> list = null;
            DeleteSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text)) commands.Add(dba.CreateCommand(text, list));
            return commands;
        }
        internal List<IADbCommand> INTERNAL_GetDeleteADbCommands(DatabaseCore accessor = null)
        {
            return GetDeleteADbCommands(accessor);
        }
        protected virtual List<IADbCommand> GetDeleteADbCommands(DatabaseCore accessor = null)
        {
            var commands = new List<IADbCommand>();
            var dba = (accessor != null) ? accessor : this.accessor;
            if (dba == null) return commands;
            if (EntityStatus < EntityState.FRESHED) return commands;

            // Delete Foreigns
            (new List<DBForeign>(Foreigns.Values)).ForEach(foreign =>
            {
                commands.AddRange(foreign.GetDeleteADbCommands(dba));
            });

            // Delete Self
            string text = null;
            List<DbParameter> list = null;
            DeleteSQL(dba, out text, out list);
            if (!string.IsNullOrWhiteSpace(text)) commands.Add(new ADbCommand(dba.CreateCommand(text, list), CallbackAction_DELETE));
            return commands;
        }

        protected virtual void CallbackAction_DELETE(DbCommand command, List<DbCommand> commands)
        {
        }
        internal DBColumn FindColumn(string column)
        {
            var id = string.IsNullOrWhiteSpace(column) ? null : column.Trim().ToUpper();
            return (id != null && Columns.ContainsKey(id)) ? Columns[id] : null;
        }
        internal List<DBColumn> GetColumns()
        {
            var columns = new List<DBColumn>();
            foreach (DBColumn column in Columns.Values) columns.Add(column);
            return columns;
        }
        #endregion


        #region Protected Methods
        protected T GetValue<T>(string column)
        {
            var value = default(T);
            if (!string.IsNullOrWhiteSpace(column))
            {
                var id = column.Trim().ToUpper();
                if (Columns.ContainsKey(id))
                {
                    value = DatabaseCore.Convert2<T>(Columns[id].Value);
                }
                else if (Foreigns.ContainsKey(id))
                {
                    value = DatabaseCore.Convert2<T>(Foreigns[id].Value);
                }
            }
            return value;
        }
        protected void SetValue(string column, object value)
        {
            if (!string.IsNullOrWhiteSpace(column))
            {
                var id = column.Trim().ToUpper();
                if (Columns.ContainsKey(id))
                {
                    Columns[id].Value = value;
                    SetEntityState();
                }
                else if (Foreigns.ContainsKey(id))
                {
                    Foreigns[id].Value = value;
                }
            }
        }
        #endregion
        

        #region Public Methods
        public int Fresh()
        {
            if (accessor == null) return 0;
            if (PrimaryList == null || PrimaryList.Count <= 0) return 0;
            string sql = null;
            List<DbParameter> parameters = null;
            FreshSQL(accessor, out sql, out parameters);
            if (string.IsNullOrWhiteSpace(sql) || parameters == null) return 0;
            var data = accessor.Retrieve(accessor.CreateCommand(sql, parameters));
            if (data == null || data.Rows.Count <= 0) return 0;
            return SetEntity(data.Rows[0], DateTime.Now.Ticks);
        }
        public long Save()
        {
            if (accessor == null) return 0;
            //return accessor.SaveTableEntity(this);
            var lst_commands = GetSaveADbCommands();
            var result = accessor.ExecuteSQLCommand(lst_commands);
            if (result >= 0)
            {
                result = Fresh();
            }
            else
            {
                if (result < 0 && accessor.LastException != null) throw accessor.LastException;
            }
            return result;
        }
        public long Delete()
        {
            if (accessor == null) return 0;
            var lst_commands = GetDeleteCommands();
            var result = accessor.ExecuteSQLCommand(lst_commands);
            if (result > 0)
            {
                this.EntityStatus = EntityState.DELETED;
            }
            else
            {
                if (result < 0 && accessor.LastException != null) throw accessor.LastException;
            }
            return result;
        }

        public DBColumn GetColumn(string column)
        {
            var col = FindColumn(column);
            return (col != null) ? col.Clone() : null;
        }
        public void SetColumn(DBColumn column)
        {
            if (column != null) SetValue(column.ID, column.Value);
        }
        public void SetColumn(string column, object value)
        {
            SetValue(column, value);
        }
        public bool ExistsBy(params string[] columns)
        {
            if (accessor == null) return false;
            if (columns == null || columns.Length <= 0) return false;

            //var where = new StringBuilder();
            //var parameters = new List<DbParameter>();
            Clause clause = null;
            new List<string>(columns).ForEach(col =>
            {
                var key = col.Trim().ToUpper();
                if (Columns.ContainsKey(key))
                {
                    var column = Columns[key];
                    if (column == null) return;

                    var id = column.ID;
                    var pid = "{" + id + "}";
                    if (clause == null)
                    {
                        clause = new Clause(string.Format("{0} = {1}", id, pid)).AddParam(id, column.Value);
                    }
                    else
                    {
                        clause.And(new Clause(string.Format("{0} = {1}", id, pid)).AddParam(id, column.Value));
                    }
                }
            });
            return accessor.CountInTable(TableName, clause) > 0;
        }

        protected abstract string CreateTableScript();
        public virtual bool CreateTable()
        {
            if (accessor == null) return false;
            var script = CreateTableScript();
            if (string.IsNullOrWhiteSpace(script)) return false;
            if (accessor.TableExists(this.TableName)) return true;

            var ret = accessor.ExecuteSQLCommand(accessor.CreateCommand(script));
            return ret >= 0;
        }

        public static T CreateEntity<T>() where T : TableEntity, new()
        {
            T entity = default(T);
            try
            {
                entity = typeof(T).Assembly.CreateInstance(typeof(T).ToString()) as T;
            }
            catch { }
            return entity;
        }
        public static V MaxValue<T, V>(DatabaseCore accessor, string column, Clause clause = null) where T : TableEntity, new()
        {
            if (accessor == null) return default(V);

            var entity = CreateEntity<T>();
            var col = entity.FindColumn(column);
            if (col == null) return default(V);
            var sql = new StringBuilder();
            sql.AppendLine(string.Format("SELECT MAX({0}) AS MAXVAL FROM [{1}]", col.ID, entity.TableName));

            string where = null;
            List<DbParameter> parameters = null;
            if (clause != null) clause.Export(accessor, out where, out parameters);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendLine("WHERE");
                sql.AppendLine(where);
            }

            return accessor.RetrieveValue<V>(accessor.CreateCommand(sql.ToString(), parameters), default(V));
        }
        public static V MinValue<T, V>(DatabaseCore accessor, string column, Clause clause = null) where T : TableEntity, new()
        {
            if (accessor == null) return default(V);

            var entity = CreateEntity<T>();
            var col = entity.FindColumn(column);
            if (col == null) return default(V);
            var sql = new StringBuilder();
            sql.AppendLine(string.Format("SELECT MIN({0}) AS MINVAL FROM [{1}]", col.ID, entity.TableName));

            string where = null;
            List<DbParameter> parameters = null;
            if (clause != null) clause.Export(accessor, out where, out parameters);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendLine("WHERE");
                sql.AppendLine(where);
            }

            return accessor.RetrieveValue<V>(accessor.CreateCommand(sql.ToString(), parameters), default(V));
        }
        public static long Count<T>(DatabaseCore accessor, Clause clause = null) where T : TableEntity, new()
        {
            if (accessor == null) return 0;

            var entity = CreateEntity<T>();
            return accessor.CountInTable(entity.TableName, clause);
        }
        #endregion


        #region Private Methods
        private void InitialColumns()
        {
            var properties = this.GetType().GetProperties();
            foreach (PropertyInfo proInfo in properties)
            {
                var attrs = proInfo.GetCustomAttributes(typeof(DBFieldAttribute), true);
                var attr = (attrs != null && attrs.Length > 0) ? (DBFieldAttribute)attrs[0] : null;
                if (attr == null || !attr.IsValid) continue;
                if (Columns.ContainsKey(attr.Name)) continue;
                Columns.Add(attr.Name, attr.CreateDBColumn());
            }

            if (Columns.Count <= 0) throw new Exception(string.Format(GeneralMessages.ERR_ENTITY_EMPTY, this.GetType().FullName, typeof(DBFieldAttribute).FullName));
        }
        private void InitialForeigns()
        {
            var properties = this.GetType().GetProperties();
            foreach (PropertyInfo proInfo in properties)
            {
                var attrs = proInfo.GetCustomAttributes(typeof(DBForeignAttribute), true);
                var attr = (attrs != null && attrs.Length > 0) ? (DBForeignAttribute)attrs[0] : null;
                if (attr == null || !attr.IsValid) continue;
                if (attr.Keys.Any(col => !Columns.ContainsKey(col.Key))) continue;
                if (Foreigns.ContainsKey(attr.TableName)) continue;

                var keys = new Dictionary<string, DBColumn>();
                attr.Keys.ForEach(col => { keys.Add(col.Value, Columns[col.Key]); });
                Foreigns.Add(attr.TableName, new DBForeign(attr.TableName, attr.Mode, keys, proInfo));
            }
        }

        private void PrimaryWhereSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null; parameters = null;
            if (accessor == null) return;
            if (PrimaryList == null || PrimaryList.Count <= 0 || PrimaryList.Any(p=>p.Value == null)) return;
            var where = new StringBuilder();
            var list = new List<DbParameter>();
            PrimaryList.ForEach(p =>
            {
                if (where.Length > 0) where.AppendLine(" AND ");
                var parameter = accessor.CreateParameter(p);
                where.Append(string.Format("{0} = {1}", p.ID, parameter.ParameterName));
                list.Add(parameter);
            });
            sql = where.ToString();
            parameters = list;
        }
        private void FreshSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null; parameters = null;

            var select = new StringBuilder();
            string where = null;
            List<DbParameter> list = null;
            PrimaryWhereSQL(accessor, out where, out list);
            if (string.IsNullOrWhiteSpace(where)) return;
            
            select.AppendLine(SQLTableSelect);
            select.AppendLine("WHERE");
            select.AppendLine(where);

            sql = select.ToString();
            parameters = list;
        }
        private void DeleteSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null; parameters = null;
            var pattern = "DELETE FROM [{0}] WHERE {1}";

            string where = null;
            List<DbParameter> list = null;
            PrimaryWhereSQL(accessor, out where, out list);
            if (string.IsNullOrWhiteSpace(where)) return;

            sql = string.Format(pattern, this.TableName, where);
            parameters = list;
        }
        private void UpdateSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null; parameters = null;
            var pattern = "UPDATE [{0}] SET {1} WHERE {2}";
            var pattern1 = "{0} = {1}";
            var list = new List<DbParameter>();
            
            var items = new List<string>();
            var columns = new List<DBColumn>(Columns.Values).FindAll(col => col.State == ColumnState.CHANGED);
            var lst_items = new List<DbParameter>();
            columns.ForEach(col =>
            {
                var parameter = accessor.CreateParameter(col);
                lst_items.Add(parameter);
                items.Add(string.Format(pattern1, col.ID, parameter.ParameterName));
            });
            if (items.Count <= 0) return;
            list.AddRange(lst_items);

            string where = null;
            List<DbParameter> lst_where = null;
            PrimaryWhereSQL(accessor, out where, out lst_where);
            if (string.IsNullOrWhiteSpace(where)) return;
            list.AddRange(lst_where);
            

            sql = string.Format(pattern, this.TableName, string.Join(",", items), where);
            parameters = list;
        }
        private void InsertSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null; parameters = null;
            var pattern = "INSERT INTO [{0}] ({1}) VALUES ({2})";

            var list = new List<DbParameter>();

            var fields = new List<string>();
            var values = new List<string>();
            var columns = new List<DBColumn>(Columns.Values);
            columns.ForEach(col =>
            {
                if (col.KeyType != KeyType.IncrementPrimary)
                {
                    var parameter = accessor.CreateParameter(col);
                    list.Add(parameter);
                    fields.Add(col.ID);
                    values.Add(parameter.ParameterName);
                }
            });

            sql = string.Format(pattern, this.TableName, string.Join(",", fields), string.Join(",", values));
            parameters = list;
        }

        private void SetEntityState()
        {
            var columns = new List<DBColumn>(Columns.Values);
            if (columns.Any(col => col.State == ColumnState.ERROR))
            {
                this.EntityStatus = EntityState.ERROR;
                return;
            }

            if (!columns.Any(col => col.State != ColumnState.RAW))
            {
                this.EntityStatus = EntityState.RAW;
                return;
            }
            if (!columns.Any(col => col.State != ColumnState.FRESHED))
            {
                this.EntityStatus = EntityState.FRESHED;
                return;
            }

            if (columns.Any(col => col.State == ColumnState.ASSIGNED))
            {
                this.EntityStatus = EntityState.ASSIGNED;
                return;
            }
            if (columns.Any(col => col.State == ColumnState.CHANGED))
            {
                this.EntityStatus = EntityState.CHANGED;
                return;
            }
            this.EntityStatus = EntityState.UNKNOWN;
        }
        #endregion


        public void Dispose()
        {
            foreach (string key in Foreigns.Keys) Foreigns[key].Dispose();
            Foreigns.Clear();
            Columns.Clear();
        }

        ~TableEntity()
        {
            Dispose();
        }
    }
}
