using Database.Commons.Objects;
using Database.Entity.Enums;
using Database.Entity.Objects;
using Database.Entity.Schemas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Database.Entity
{
    internal class DBForeign
    {
        internal Guid ID { get; private set; }

        private TableSchema _host = null;
        //private TableEntity _host = null;
        private PropertyInfo _pinfo = null;

        public Type Type
        {
            get
            {
                return _pinfo.PropertyType;
            }
        }
        public Type EntityType
        {
            get
            {
                return Type.GetGenericArguments().First();
            }
        }
        public TableEntity EntityInst
        {
            get
            {
                return EntityType.Assembly.CreateInstance(EntityType.FullName) as TableEntity;
            }
        }
        public DBRefBase Value
        {
            get
            {
                return _pinfo.GetValue(_host.Entity) as DBRefBase;
            }
        }
        public STATUS Status
        {
            get
            {
                var value = Value;
                return value != null ? value.Status : STATUS.RAW;
            }
        }

        public string Name { get; private set; }

        private IList<ReadOnlyPair<string, string>> _keys { get; set; }
        public ReadOnlyPair<string, string>[] Keys
        {
            get
            {
                return _keys.ToArray();
            }
        }

        internal DBForeign(TableSchema host, PropertyInfo pinfo, ReadOnlyPair<string, string>[] keys, bool initial)
        {
            if (host == null) throw new ArgumentNullException("DBForeign(host) is null!");
            if (pinfo == null) throw new ArgumentNullException("DBForeign(pinfo) is null!");
            if (keys == null || keys.Length <= 0) throw new ArgumentNullException("DBForeign(keys) is null or empty!");

            ID = Guid.NewGuid();

            _host = host;
            _pinfo = pinfo;
            _keys = new List<ReadOnlyPair<string, string>>();

            CheckType();
            CheckKeys(keys);

            Name = _pinfo.Name;
            if (initial) _pinfo.SetValue(_host.Entity, Type.Assembly.CreateInstance(Type.FullName));
        }

        internal void Load(DataTable table, DataSet dbset, int flag, int level)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (dbset == null) throw new ArgumentNullException("dbset");

            var value = Value;
            if (value == null)
            {
                value = Type.Assembly.CreateInstance(Type.FullName) as DBRefBase;
                _pinfo.SetValue(_host.Entity, value);
            }
            value.Clear();
            var rows = table.Rows.Cast<DataRow>().Where(row =>
            {
                var result = 1;
                _keys.ToList().ForEach(k =>
                {
                    var column = _host.GetColumn(k.Key);
                    var pk0 = column.Value;
                    var pk1 = TableSchema.ValueTypeConvert(row[k.Value], column.Type);
                    result *= pk0.Equals(pk1) ? 1 : 0;
                });
                return result == 1;
            });


            if (rows.Count() > 0)
            {
                if (value.Type.Equals("DBRefEntity"))
                {
                   var ent = value.NewTableEntity();
                   ent.Fill(rows.First(), dbset, flag, level);
                }

                if (value.Type.Equals("DBRefList"))
                {
                    rows.Cast<DataRow>().ToList().ForEach(row =>
                    {
                        var ent = value.NewTableEntity();
                        ent.Fill(row, dbset, flag, level);
                    });


                }
            }
        }
        internal void Save(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            var value = Value;
            if (value != null) value.Save(collection);
        }

        internal void Delete(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            var value = Value;
            if (value != null) value.Delete(collection);
        }

        private void CheckType()
        {
            var type = _pinfo.PropertyType;
            Func<Type, string> func_name = (t) => string.Format("{0}.{1}", t.Namespace, t.Name);
            var tnm = func_name(type);

            if (!tnm.Equals(func_name(typeof(DBRefEntity<TableEntity>))) && !tnm.Equals(func_name(typeof(DBRefList<TableEntity>))))
            {
                throw new ApplicationException(string.Format("DBForeign({0}) has illegal type!", type.FullName));
            }
        }
        private void CheckKeys(ReadOnlyPair<string, string>[] keys)
        {
            //Check Key1
            CheckKeys(_host, keys.Select(k => k.Key).ToArray());
            //Check Key2
            CheckKeys(EntityInst.Schema, keys.Select(k => k.Value).ToArray());

            (_keys as List<ReadOnlyPair<string, string>>).AddRange(keys);
        }
        private void CheckKeys(TableSchema schema, string[] keys)
        {
            if (keys != null && keys.Any(k => !schema.ColumnNames.Any(c => c.Equals(k))))
            {
                var key = keys.First(k => !schema.ColumnNames.Any(c => c.Equals(k)));
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ApplicationException(string.Format("DBForeign({0}) has empty foreign column name!", schema.Entity.GetType().FullName));
                }
                else
                {
                    throw new ApplicationException(string.Format("DBForeign({0}) has unexisted foreign column name({1})!", schema.Entity.GetType().FullName, key));
                }
            }
        }
    }
}
