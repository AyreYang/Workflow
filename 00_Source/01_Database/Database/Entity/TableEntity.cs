using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Entity.Schemas;
using Database.Interfaces;
using System;
using System.Data;
using System.Linq;

namespace Database.Entity
{
    public abstract class TableEntity : Entity, IDisposable
    {
        internal TableSchema Schema { get; private set; }

        public TableEntity()
        {
            Schema = new TableSchema(this);
        }

        public void Save(IDatabaseAccessor accessor, bool fresh = false)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            var collection = new SQLScriptCollection(accessor);
            this.Schema.FillSaveScriptCollection(collection);
            accessor.ExecuteSQLCommand(collection.ExportCommands());
            if (fresh) this.Fresh(accessor);
        }
        public void Delete(IDatabaseAccessor accessor)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            var collection = new SQLScriptCollection(accessor);
            this.Schema.FillDeleteScriptCollection(collection);
            accessor.ExecuteSQLCommand(collection.ExportCommands());
        }
        public void Fresh(IDatabaseAccessor accessor)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            var dbset = RetrieveDataSet(accessor);
            var table = dbset.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName.Equals(0.ToString()));
            if (table == null) throw new ApplicationException(string.Format("Missing table({0})!", Schema.Name));
            if (table.Rows.Count <= 0) throw new ApplicationException(string.Format("No data in table({0})!", Schema.Name));
            Fill(table.Rows[0], dbset, 0, 0);
        }
        internal void Fill(DataRow row, DataSet dbset, int flag, int level)
        {
            if (row == null) throw new ArgumentNullException("row");
            if (dbset == null) throw new ArgumentNullException("dbset");

            this.LoadFrom(row, 0);

            this.Schema.FillForeigns(dbset, 0, level);
        }

        private DataSet RetrieveDataSet(IDatabaseAccessor accessor)
        {
            var collection = new SQLScriptCollection(accessor);
            Schema.FillFreshScriptCollection(collection);
            var dbset = accessor.RetrieveDataSet(collection.ExportCommand());
            if (dbset.Tables.Count > 0)
            {
                for (var i = 0; i < dbset.Tables.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(collection.Scripts[i].ID)) continue;
                    dbset.Tables[i].TableName = collection.Scripts[i].ID.ToString();
                }
            }
            return dbset;
        }

        protected override void SetValue(string name, object value, int flag = 1)
        {
            Schema.SetColumnValue(name, value, flag);
        }
        protected virtual Clause Filter(TableItem item, SQLParamCreater creater)
        {
            return null;
        }

        internal void AddJoinFilter(JoinItem join, SQLParamCreater creater)
        {
            var filter = Filter(join, creater);
            if (filter != null)
            {
                join.ON(filter);
            }
            
        }
        internal void AddWhereFilter(WhereItem where, SQLParamCreater creater)
        {
            var filter = Filter(where.TableItem, creater);
            if (filter != null)
            {
                where.And(filter);
            }

        }


        public void Dispose()
        {
            Schema.Dispose();
        }
    }
}
