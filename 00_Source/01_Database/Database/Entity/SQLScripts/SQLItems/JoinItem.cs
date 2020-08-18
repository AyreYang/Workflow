using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Database.Entity.SQLScripts.SQLItems
{
    public class JoinItem<T> : JoinItem
        where T : TableEntity
    {
        internal T Entity { get; private set; }
        private IList<KeyValuePair<string, string>> _keys { get; set; }
        public KeyValuePair<string, string>[] Keys
        {
            get
            {
                return _keys.ToArray();
            }
        }

        private JoinItem(IDatabaseAccessor accessor, T entity, string alias, KeyValuePair<string, string>[] keys, int flag, Clause clause)
            : base(accessor, entity.Schema.Name, alias, flag, clause, entity.Schema.ColumnNames)
        {
            Entity = entity;
            _keys = keys == null ? new List<KeyValuePair<string, string>>() : new List<KeyValuePair<string, string>>(keys);
        }

        //private JoinItem(IDatabaseAccessor accessor, string table, string alias, KeyValuePair<string, string>[] keys, int flag, Clause clause, string[] columns)
        //    :base(accessor, table, alias, flag, clause, columns)
        //{
        //    _keys = keys == null ? new List<KeyValuePair<string, string>>() : new List<KeyValuePair<string, string>>(keys);
        //}
        //protected JoinItem(IDatabaseAccessor accessor, T entity = null, string alias = null, KeyValuePair<string, string>[] keys = null, int flag = 0, Clause clause = null)
        //    : this(
        //            accessor,
        //            entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as T),
        //            alias,
        //            keys,
        //            flag,
        //            clause
        //            )
        //{
        //    _keys = keys == null ? new List<KeyValuePair<string, string>>() : new List<KeyValuePair<string, string>>(keys);
        //}
        public JoinItem(IDatabaseAccessor accessor, T entity = null, string alias = null, KeyValuePair<string, string>[] keys = null, int flag = 0) 
            : this(accessor, entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as T), alias, keys, flag, null)
        {
        }
        public JoinItem(IDatabaseAccessor accessor, T entity = null, KeyValuePair<string, string>[] keys = null)
            : this(accessor, entity != null ? entity : (typeof(T).Assembly.CreateInstance(typeof(T).FullName) as T), null, keys, 0) { }

        public override object Clone()
        {
            return new JoinItem<T>(_accessor, Entity, _alias, _keys.ToArray(), _flag, _clause);
        }
    }
}
