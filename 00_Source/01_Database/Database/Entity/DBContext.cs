using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Entity.SQLScripts.SQLItems;
using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entity
{
    public class DBContext
    {
        public IDatabaseAccessor Accessor { get; private set; }

        public DBContext(IDatabaseAccessor accessor)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            Accessor = accessor;
        }

        public DBRefList<T> Retrieve<T>(Clause clause = null) where T : TableEntity
        {
            var script = new SQLScript(Accessor, Commons.Objects.Enums.SQLTYPE.SELECT);
            var from = new FromItem<T>(Accessor);
            var select = new SelectItem(Accessor, from);
            script.AddItems(select, from);
            if (clause != null)
            {
                var where = new WhereItem(Accessor, clause, from);
                script.AddItems(where);
            }

            var list = new DBRefList<T>();

            var table = Accessor.Retrieve(script.ExportCommand());
            if(table != null && table.Rows.Count > 0)
            {
                foreach(var row in table.Rows)
                {
                    list.NewEntity().LoadFrom(row as DataRow, 0);
                }
            }
            return list;
        }
    }
}
