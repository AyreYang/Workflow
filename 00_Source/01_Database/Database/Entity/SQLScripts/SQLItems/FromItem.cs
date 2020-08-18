using Database.Commons.Objects.SQLItems;
using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entity.SQLScripts.SQLItems
{
    public class FromItem<T> : FromItem
        where T : TableEntity
    {
        public FromItem(IDatabaseAccessor accessor, T entity = null, string alias = null) : base(
            accessor,
            (entity != null ? entity : (Activator.CreateInstance(typeof(T)) as TableEntity)).Schema.Name,
            alias,
            (entity != null ? entity : (Activator.CreateInstance(typeof(T)) as TableEntity)).Schema.ColumnNames
            )
        { }
    }
}
