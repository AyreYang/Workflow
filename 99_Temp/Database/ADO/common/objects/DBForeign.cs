using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using DataBase.common.enums;
using DataBase.common.messages;
using Database.ADO.interfaces;

namespace DataBase.common.objects
{
    internal class DBForeign : IDisposable
    {
        public string TableName { get; private set; }
        public ForeignMode Mode { get; private set; }
        public Dictionary<string, DBColumn> Keys { get; private set; }

        public Type EntityType { get; private set; }
        public Type ValueType { get; private set; }
        public int ForeignType = 0;    //0:none 1:entity 2:list
        private int GetForeignType()
        {
            var type = 0;
            if (ValueType.FullName.StartsWith("System.Collections.Generic.List"))
            {
                type = 2;
            }
            else
            {
                var obj = ValueType.Assembly.CreateInstance(ValueType.ToString());
                type = (obj is TableEntity) ? 1 : 0;
            }
            return type;
        }
        private bool IsFromTableEntity
        {
            get
            {
                //var result = EntityType.IsSubclassOf(typeof(TableEntity));
                //var ent = EntityType.Assembly.CreateInstance(EntityType.ToString());
                return EntityType.IsSubclassOf(typeof(TableEntity));
            }
        }

        public object Value { get; set; }

       
        public DBForeign(string table, ForeignMode mode, Dictionary<string, DBColumn> primaries, PropertyInfo info)
        {
            if (string.IsNullOrWhiteSpace(table)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "table"));
            if (primaries == null || primaries.Count <= 0) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL, "primaries"));
            if (info == null) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL, "info"));

            this.TableName = table.Trim();
            this.Mode = mode;
            this.Keys = primaries;

            this.ValueType = info.PropertyType;
            var types = this.ValueType.GetGenericArguments();
            this.EntityType = (types == null || types.Length <= 0) ? this.ValueType : types[0];

            ForeignType = GetForeignType();
            if (ForeignType == 0) throw new Exception(string.Format(GeneralMessages.ERR_FOREIGN_TYPE_INVALID, info.Name, ValueType.FullName));
            if (ForeignType == 2 && !IsFromTableEntity) throw new Exception(string.Format(GeneralMessages.ERR_FOREIGN_ENTITY_TYPE_INVALID, info.Name, EntityType.FullName));
        }

        public void Fresh(DatabaseCore accessor)
        {
            if (accessor == null) return;
            var data = Fetch(accessor);
            if (data != null) Value = data;
        }
        public object Fetch(DatabaseCore accessor)
        {
            if (accessor == null) return null;

            object result = null;
            string sql = null;
            List<DbParameter> parameters = null;
            TableEntity ent = null;
            FreshSQL(accessor, out sql, out parameters);
            var data = accessor.Retrieve(accessor.CreateCommand(sql, parameters));
            if (data != null && data.Rows.Count > 0)
            {
                result = CreateObject();
                switch (ForeignType)
                {
                    case 1: //Entity
                        ent = (TableEntity)result;
                        ent.SetDBAccessor(accessor);
                        ent.SetEntity(data.Rows[0]);
                        ent.Fresh();
                        break;
                    case 2: //List
                        foreach (DataRow row in data.Rows)
                        {
                            ent = CreateEntity();
                            ent.SetDBAccessor(accessor);
                            ent.SetEntity(row);
                            ent.Fresh();
                            AddEntity2List(ent, result);
                        }
                        break;
                }
            }
            return result;
        }

        private void FreshSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null;
            parameters = null;
            if (accessor == null) return;
            var ent = CreateEntity();

            if (ent == null || Keys == null || Keys.Count <= 0) return;

            var select = new StringBuilder();
            var where = new StringBuilder();
            var list = new List<DbParameter>();
            foreach (string key in Keys.Keys)
            {
                if (where.Length > 0) where.AppendLine("AND");
                var parameter = accessor.CreateParameter(Keys[key]);
                where.AppendLine(string.Format("{0} = {1}", key, parameter.ParameterName));
                list.Add(parameter);
            }

            select.AppendLine(ent.SQLTableSelect);
            if (where.Length > 0)
            {
                select.AppendLine("WHERE");
                select.AppendLine(where.ToString());
            }

            parameters = list;
            sql = select.ToString();
        }

        private void DeleteSQL(DatabaseCore accessor, out string sql, out List<DbParameter> parameters)
        {
            sql = null;
            parameters = null;
            if (accessor == null) return;
            

            if (Keys == null || Keys.Count <= 0) return;
            var temp = string.Format("DELETE FROM {0}", TableName);

            var select = new StringBuilder();
            var where = new StringBuilder();
            var list = new List<DbParameter>();
            foreach (string key in Keys.Keys)
            {
                if (where.Length > 0) where.AppendLine("AND");
                var parameter = accessor.CreateParameter(Keys[key]);
                where.AppendLine(string.Format("{0} = {1}", key, parameter.ParameterName));
                list.Add(parameter);
            }

            select.AppendLine(temp);
            if (where.Length > 0)
            {
                select.AppendLine("WHERE");
                select.AppendLine(where.ToString());
            }

            parameters = list;
            sql = select.ToString();
        }

        private object CreateObject()
        {
            object val = null;
            switch (ForeignType)
            {
                case 1: //Entity
                    val = ValueType.Assembly.CreateInstance(ValueType.ToString());
                    break;
                case 2: //List
                    var list = typeof(List<>);
                    var type = list.MakeGenericType(new Type[] { EntityType });
                    val = Activator.CreateInstance(type);
                    break;
            }
            return val;
        }
        private TableEntity CreateEntity()
        {
            object val = EntityType.Assembly.CreateInstance(EntityType.ToString());
            return val == null ? null : (TableEntity)val;
        }

        private void AddEntity2List(object entity, object list)
        {
            if (list == null || entity == null) return;

            //if (Value == null) Value = CreateObject();
            var listType = typeof(List<>).MakeGenericType(EntityType);
            //var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");
            addMethod.Invoke(list, new object[] { entity });
            //var count = listType.GetProperty("Count").GetValue(Value, null);
            return;
        }

        public List<DbCommand> GetDeleteCommands(DatabaseCore accessor)
        {
            var commands = new List<DbCommand>();
            if (accessor != null && Mode == ForeignMode.Correlative && Value != null)
            {
                switch (ForeignType)
                {
                    case 1: //Entity
                        commands.AddRange(((TableEntity)Value).INTERNAL_GetDeleteCommands(accessor));
                        break;
                    case 2: //List
                        string sql = string.Empty;
                        List<DbParameter> parameters = null;
                        DeleteSQL(accessor, out sql, out parameters);
                        var command = accessor.CreateCommand(sql, parameters);
                        commands.Add(command);
                        break;
                }
            }
            return commands;
        }
        public List<IADbCommand> GetDeleteADbCommands(DatabaseCore accessor, Action<DbCommand, List<DbCommand>> action = null)
        {
            var commands = new List<IADbCommand>();
            if (accessor != null && Mode == ForeignMode.Correlative && Value != null)
            {
                switch (ForeignType)
                {
                    case 1: //Entity
                        commands.AddRange(((TableEntity)Value).INTERNAL_GetDeleteADbCommands(accessor));
                        break;
                    case 2: //List
                        string sql = string.Empty;
                        List<DbParameter> parameters = null;
                        DeleteSQL(accessor, out sql, out parameters);
                        var command = new ADbCommand(accessor.CreateCommand(sql, parameters), action);
                        commands.Add(command);
                        break;
                }
            }
            return commands;
        }

        public List<DbCommand> GetSaveCommands(DatabaseCore accessor)
        {
            var commands = new List<DbCommand>();
            if (accessor != null && Mode == ForeignMode.Correlative && Value != null)
            {
                switch (ForeignType)
                {
                    case 1: //Entity
                        commands.AddRange(((TableEntity)Value).INTERNAL_GetSaveCommands(accessor));
                        break;
                    case 2: //List
                        var data0 = Fetch(accessor) as IEnumerable<dynamic>;
                        var lst_data0 = (data0 != null) ? new List<dynamic>(data0) : new List<dynamic>();
                        var data1 = Value as IEnumerable<dynamic>;
                        var lst_data1 = (data1 != null) ? new List<dynamic>(data1) : new List<dynamic>();

                        // Find deleted entities.
                        var lst_del = lst_data0.FindAll(itm0 => !lst_data1.Any(itm1 => itm1.PrimaryKeys.Equals(itm0.PrimaryKeys)));
                        // Find new entities.
                        var lst_new = lst_data1.FindAll(itm1 => !lst_data0.Any(itm0 => itm0.PrimaryKeys.Equals(itm1.PrimaryKeys)));
                        // Find existed entities
                        var lst_existed = lst_data0.FindAll(itm0 => lst_data1.Any(itm1 => itm1.PrimaryKeys.Equals(itm0.PrimaryKeys)));

                        lst_del.ForEach(itm =>
                        {
                            commands.AddRange(itm.INTERNAL_GetDeleteCommands(accessor));
                        });
                        lst_new.ForEach(itm =>
                        {
                            commands.AddRange(itm.INTERNAL_GetSaveCommands(accessor));
                        });
                        lst_existed.ForEach(itm =>
                        {
                            commands.AddRange(itm.INTERNAL_GetSaveCommands(accessor));
                        });
                        break;
                }
            }
            return commands;
        }
        public List<IADbCommand> GetSaveADbCommands(DatabaseCore accessor)
        {
            var commands = new List<IADbCommand>();
            if (accessor != null && Mode == ForeignMode.Correlative && Value != null)
            {
                switch (ForeignType)
                {
                    case 1: //Entity
                        commands.AddRange(((TableEntity)Value).INTERNAL_GetSaveADbCommands(accessor));
                        break;
                    case 2: //List
                        var data0 = Fetch(accessor) as IEnumerable<dynamic>;
                        var lst_data0 = (data0 != null) ? new List<dynamic>(data0) : new List<dynamic>();
                        var data1 = Value as IEnumerable<dynamic>;
                        var lst_data1 = (data1 != null) ? new List<dynamic>(data1) : new List<dynamic>();

                        // Find deleted entities.
                        var lst_del = lst_data0.FindAll(itm0 => !lst_data1.Any(itm1 => itm1.PrimaryKeys.Equals(itm0.PrimaryKeys)));
                        // Find new entities.
                        var lst_new = lst_data1.FindAll(itm1 => !lst_data0.Any(itm0 => itm0.PrimaryKeys.Equals(itm1.PrimaryKeys)));
                        // Find existed entities
                        var lst_existed = lst_data0.FindAll(itm0 => lst_data1.Any(itm1 => itm1.PrimaryKeys.Equals(itm0.PrimaryKeys)));

                        lst_del.ForEach(itm =>
                        {
                            commands.AddRange(itm.INTERNAL_GetDeleteADbCommands(accessor));
                        });
                        lst_new.ForEach(itm =>
                        {
                            commands.AddRange(itm.INTERNAL_GetSaveADbCommands(accessor));
                        });
                        lst_existed.ForEach(itm =>
                        {
                            commands.AddRange(itm.INTERNAL_GetSaveADbCommands(accessor));
                        });
                        break;
                }
            }
            return commands;
        }


        public void Dispose()
        {
            Keys.Clear();
        }

        ~DBForeign()
        {
            Dispose();
        }
    }
}
