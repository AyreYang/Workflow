using Database.Commons.Objects;
using Database.Commons.Objects.Enums;
using Database.Commons.Objects.SQLItems;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using Database.Entity.SQLScripts.SQLItems;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Database.Entity.Schemas
{
    internal class TableSchema : IDisposable
    {
        public TableEntity Entity { get; private set; }
        public bool Ready { get; private set; }

        public bool ReadOnly { get; private set; }
        public string Name { get; set; }
        private IList<DBColumn> Columns { get; set; }
        public string[] ColumnNames
        {
            get
            {
                return Columns.Select(c => c.Name).ToArray();
            }
        }
        private IList<DBForeign> Foreigns { get; set; }
        public string[] ForeignNames
        {
            get
            {
                return Foreigns.Select(f => f.Name).ToArray();
            }
        }
        public bool HasForeigns { get { return ForeignNames.Length > 0; } }
        private STATUS ColumnsStatus
        {
            get
            {
                return (STATUS)Columns.Max(c => (int)c.Status);
            }
        }
        private STATUS ForeignsStatus
        {
            get
            {
                return HasForeigns ? (STATUS)Foreigns.Max(f => (int)f.Status) : STATUS.RAW;
            }
        }

        public STATUS Status
        {
            get
            {
                return (STATUS)Math.Max((int)ColumnsStatus, (int)ForeignsStatus);
            }
        }

        public TableSchema(TableEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            Entity = entity;
            Columns = new List<DBColumn>();
            Foreigns = new List<DBForeign>();
            SetTable();
            InitialColumns();
            InitialForeigns();
            Ready = true;
        }

        #region Private Initial Methods
        private void SetTable()
        {
            var attr = Entity.GetType().GetCustomAttribute(typeof(DBTableAttribute)) as DBTableAttribute;
            if (attr != null)
            {
                Name = attr.Name;
                ReadOnly = attr.Readonly;
            }
            else
            {
                Name = Entity.GetType().Name;
                ReadOnly = false;
            }
        }
        private void InitialColumns()
        {
            var properties = Entity.GetType().GetProperties().ToList().FindAll(p => p.GetCustomAttributes(typeof(DBColumnAttribute), true).Length > 0);
            foreach (PropertyInfo proInfo in properties)
            {
                var attr = proInfo.GetCustomAttributes(typeof(DBColumnAttribute), true).First() as DBColumnAttribute;
                if (Columns.Any(c => c.Name.Equals(proInfo.Name))) continue;
                var column = new DBColumn(Entity, proInfo, attr.Type, attr.Size1, attr.Size2, attr.IsPK, attr.Nullable, attr.FuncDefaultValue);
                Columns.Add(column);
            }

            if (Columns.Count <= 0) throw new ApplicationException(string.Format("The TableEntity({0}) has no DBColumnAttribute definitions!", this.GetType().FullName));
            if (!Columns.Any(c => c.IsPK)) throw new ApplicationException(string.Format("The TableEntity({0}) has no primary key columns!", this.GetType().FullName));
        }
        private void InitialForeigns()
        {
            var properties = Entity.GetType().GetProperties().ToList().FindAll(p => p.GetCustomAttributes(typeof(DBForeignAttribute), true).Length > 0);
            foreach (PropertyInfo proInfo in properties)
            {
                if (ForeignNames.Any(f => f.Equals(proInfo.Name))) continue;

                var type = proInfo.PropertyType;
                var attr = proInfo.GetCustomAttributes(typeof(DBForeignAttribute), true).First() as DBForeignAttribute;
                if (attr == null) continue;
                var foreign = new DBForeign(this, proInfo, attr.Keys, attr.Initial);
                Foreigns.Add(foreign);
            }
        }
        #endregion

        #region Fill SQLScriptCollection Methods
        internal void FillFreshScriptCollection(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            collection.Clear();
            FillSelectScript(null, 0, collection);
        }
        internal void FillSaveScriptCollection(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            collection.Clear();
            FillSaveScript(collection);
        }
        internal void FillDeleteScriptCollection(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            collection.Clear();
            FillDeleteScript(collection);
        }
        #endregion

        private void FillSelectScript(string name, int level, SQLScriptCollection collection, WhereItem where = null, Stack<JoinItem<TableEntity>> stack = null)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            var script = collection.NewSQLScript(level == 0 ? 0.ToString() : string.Format("{0}-{1}", name.Trim(), level), SQLTYPE.SELECT);
            var alias = collection.NewTableAlias();
            var from = new FromItem<TableEntity>(collection.accessor, Entity, alias);
            var select = new SelectItem(collection.accessor, from);
            
            script.AddItems(select, from);

            //Build Jions
            JoinItem<TableEntity>[] join_items = null;
            if (stack != null)
            {
                if (HasForeigns)
                {
                    join_items = new JoinItem<TableEntity>[stack.Count];
                    stack.CopyTo(join_items, 0);
                }

                TableItem table1 = from;
                JoinItem<TableEntity> table2 = null;
                while (stack.Count > 0)
                {
                    table2 = stack.Pop().Clone() as JoinItem<TableEntity>;
                    table2.ON(table1, table2.Keys.ToArray());
                    table2.Entity.AddJoinFilter(table2, collection.SQLParamCreater);
                    script.AddItems(table2);
                    table1 = table2;
                }
            }

            //Build Where
            if (level == 0)
            {
                var where1 = new WhereItem(collection.accessor, from);
                Columns.Where(c => c.IsPK).ToList().ForEach(col =>
                {
                    var param = collection.NewParameter(col.Value);
                    where1.And(where1.ColumnEquals(col.Name, param));
                });
                where = where == null ? where1 : where + where1;
            }
            if (where != null) script.AddItem(where.Clone() as WhereItem);
            var where0 = new WhereItem(collection.accessor, from);
            Entity.AddWhereFilter(where0, collection.SQLParamCreater);
            script.AddItem(where0);

            foreach (var foreign in this.Foreigns)
            {
                var nstack = join_items != null ? new Stack<JoinItem<TableEntity>>(join_items) : new Stack<JoinItem<TableEntity>>();
                nstack.Push(new JoinItem<TableEntity>(collection.accessor, Entity, alias, foreign.Keys.Select(key => key.ToKeyValuePair()).ToArray()));
                foreign.EntityInst.Schema.FillSelectScript(foreign.Name, level + 1, collection, where.Clone() as WhereItem, nstack);
            }

        }
        internal void FillSaveScript(SQLScriptCollection collection)
        {
            switch (this.ColumnsStatus)
            {
                case STATUS.ASSIGNED:
                case STATUS.RAW:
                    var script1 = collection.NewSQLScript(null, SQLTYPE.INSERT);
                    var insert = new InsertItem<TableEntity>(collection.accessor, Entity);
                    Columns.ToList().ForEach(col =>
                    {
                        var param = collection.NewParameter(col.Name, col.Value);
                        insert.Append(new Clause(script1.accessor, param.CoveredName, new KeyValuePair<string, object>[] { param.ToKeyValuePair() }));
                    });
                    script1.AddItem(insert);
                    break;
                case STATUS.CHANGED:
                    var script2 = collection.NewSQLScript(null, SQLTYPE.UPDATE);
                    var update = new UpdateItem<TableEntity>(collection.accessor, Entity);
                    Columns.Where(c => c.Status == STATUS.CHANGED).ToList().ForEach(col =>
                    {
                        var param = collection.NewParameter(col.Name, col.Value);
                        update.Append(new Clause(script2.accessor, string.Format("{0} = {1}", col.Name, param.CoveredName), new KeyValuePair<string, object>[] { param.ToKeyValuePair() }));
                    });
                    script2.AddItem(update);
                    var where = new WhereItem(script2.accessor, update);
                    Columns.Where(c => c.IsPK).ToList().ForEach(col =>
                    {
                        var param = collection.NewParameter(col.Name, col.Value);
                        where.And(where.ColumnEquals(col.Name, param));
                    });
                    script2.AddItem(where);
                    break;
            }

            foreach (var foreign in this.Foreigns)
            {
                foreign.Save(collection);
            }
        }
        internal void FillDeleteScript(SQLScriptCollection collection)
        {
            var script = collection.NewSQLScript(null, SQLTYPE.DELETE);
            var delete = new DeleteItem<TableEntity>(collection.accessor, Entity);
            script.AddItem(delete);
            var where = new WhereItem(script.accessor, delete);
            Columns.Where(c => c.IsPK).ToList().ForEach(col =>
            {
                var param = collection.NewParameter(col.Name, col.Value);
                where.And(where.ColumnEquals(col.Name, param));
            });
            script.AddItem(where);

            if (this.HasForeigns)
            {
                foreach (var foreign in this.Foreigns)
                {
                    foreign.Delete(collection);
                }
            }
        }

        private DBColumn GetColumn(string col)
        {
            if (string.IsNullOrWhiteSpace(col)) throw new ArgumentNullException("col");
            return Columns.FirstOrDefault(c => c.Name.ToUpper().Equals(col.Trim().ToUpper()));
        }
        internal void SetColumnValue(string col, object val, int flag)
        {
            var column = GetColumn(col);
            if (column != null)
            {
                if (flag == 0) column.SetValue0(val);
                column.SetValue(val);
            }
        }
        internal object GetColumnValue(string col)
        {
            var column = GetColumn(col);
            return (column != null) ? column.Value : null;
        }

        internal void FillForeigns(DataSet dbset, int flag, int level)
        {
            if (HasForeigns)
            {
                foreach(var foreign in Foreigns)
                {
                    var id = string.Format("{0}-{1}", foreign.Name, level + 1);
                    var table = dbset.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName.Equals(id));
                    foreign.Load(table, dbset, flag, level + 1);
                }
            }
        }
        public void Dispose()
        {
            Ready = false;
            if (Columns != null) Columns.Clear();
            Columns = null;
        }
    }
}
