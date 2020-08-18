using Database.Commons.Objects.SQLItems;
using Database.Interfaces;

namespace Database.Entity.SQLScripts.SQLItems
{
    public class UpdateItem<T> : UpdateItem
        where T : TableEntity
    {
        public UpdateItem(IDatabaseAccessor accessor, T entity = null) : base(
            accessor,
            (entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as TableEntity)).Schema.Name,
            null,
            (entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as TableEntity)).Schema.ColumnNames
            )
        { }
    }
}
