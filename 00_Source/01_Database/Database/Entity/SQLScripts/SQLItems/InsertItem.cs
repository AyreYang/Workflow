using Database.Commons.Objects.SQLItems;
using Database.Interfaces;

namespace Database.Entity.SQLScripts.SQLItems
{
    public class InsertItem<T> : InsertItem
        where T : TableEntity
    {
        public InsertItem(IDatabaseAccessor accessor, T entity = null) : base(
            accessor,
            (entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as TableEntity)).Schema.Name,
            null,
            (entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as TableEntity)).Schema.ColumnNames
            )
        { }
    }
}
