using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Database.Entity
{
    public abstract class Entity
    {
        struct ConverterItem
        {
            public string key { get; set; }
            public Func<object, object> converter { get; set; }
        }

        private IList<ConverterItem> _converters { get; set; }

        public Entity()
        {
            _converters = new List<ConverterItem>();

            SetConverters(AddConverter);
        }

        public void Load<T>(T source) where T : class
        {
            if (source is DataRow)
            {
                LoadFrom(source as DataRow);
            }
            else
            {
                LoadFrom<T>(source);
            }
        }

        protected virtual void PostSetValue(string name, object value, int flag) { }

        internal void LoadFrom(DataRow row, int flag = 1)
        {
            if (row == null) return;
            var columns = row.Table.Columns;
            if (columns == null || columns.Count <= 0) return;

            for (var i = 0; i < columns.Count; i++)
            {
                var key = (columns[i].ColumnName ?? string.Empty).Trim();
                var value = row[i];
                if (value == DBNull.Value) value = null;

                SetValue(key, value, flag);
            }
        }
        internal void LoadFrom<T>(T obj, int flag = 1) where T : class
        {
            if (obj == null) return;
            var properties = obj.GetType().GetProperties().Where(p => p.CanRead).ToList();
            foreach (var property in properties)
            {
                var key = (property.Name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(key)) continue;
                var value = property.GetValue(obj, null);
                value = _converters.Any(m => m.key.Equals(key)) ?
                    _converters.First(m => m.key.Equals(key)).converter(value) : value;
                SetValue(key, value, flag);

                PostSetValue(key, value, flag);
            }
        }

        protected virtual void SetValue(string name, object value, int flag = 1)
        {
            var property = this.GetType().GetProperty(name);
            if (property != null) property.SetValue(this, value, null);
        }


        protected virtual void SetConverters(Action<string, Func<object, object>> action)
        {
            _converters.Clear();
        }

        private void AddConverter(string key, Func<object, object> converter)
        {
            if (string.IsNullOrWhiteSpace(key) || converter == null) return;
            var fkey = key.Trim();

            if (_converters.Any(m => m.key.Equals(fkey)))
            {
                var item = _converters.First(m => m.key.Equals(fkey));
                item.converter = converter;
            }
            else
            {
                _converters.Add(new ConverterItem() { key = fkey, converter = converter });
            }
        }


        

        public static List<T> ToList<T>(DataTable table, Func<T, bool> func = null) where T : Entity, new()
        {
            if (table == null) return null;

            var list = new List<T>();
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                {
                    var vw = new T();
                    vw.Load<DataRow>(row as DataRow);
                    if (func == null || func(vw)) list.Add(vw);
                }
            }
            return list;
        }
        public static List<T> ToList<T>(DataSet dset, Func<T, bool> func = null) where T : Entity, new()
        {
            return ToList<T>(typeof(T).Name, dset, func);
        }
        public static List<T> ToList<T>(string key, DataSet dset, Func<T, bool> func = null) where T : Entity, new()
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (dset == null) return null;
            if (!dset.Tables.Contains(key.Trim())) return null;
            var table = dset.Tables[key.Trim()];

            var list = new List<T>();
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                {
                    var vw = new T();
                    vw.Load<DataRow>(row as DataRow);
                    if (func == null || func(vw))
                    {
                        vw.LoadMore(dset);
                        list.Add(vw);
                    }
                }
            }
            return list;
        }

        public static T ToFirst<T>(DataTable table, Func<T, bool> func = null) where T : Entity, new()
        {
            if (table == null) return null;

            T t = default(T);
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                {
                    var vw = new T();
                    vw.Load<DataRow>(row as DataRow);
                    if (func == null || func(vw))
                    {
                        t = vw;
                        break;
                    }
                }
            }
            return t;
        }
        public static T ToFirst<T>(DataSet dset, Func<T, bool> func = null) where T : Entity, new()
        {
            return ToFirst<T>(typeof(T).Name, dset, func);
        }
        public static T ToFirst<T>(string key, DataSet dset, Func<T, bool> func = null) where T : Entity, new()
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (dset == null) return null;
            if (!dset.Tables.Contains(key.Trim())) return null;
            var table = dset.Tables[key.Trim()];

            var t = default(T);
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                {
                    var vw = new T();
                    vw.Load<DataRow>(row as DataRow);
                    if (func == null || func(vw))
                    {
                        vw.LoadMore(dset);
                        t = vw;
                        break;
                    }
                }
            }
            return t;
        }

        public virtual void LoadMore(DataSet dset)
        {
            //if (dset == null || func == null || !dset.Tables.Cast<DataTable>().Any(t => func(t))) return;
            //LoadMore(dset.Tables.Cast<DataTable>().First(t => func(t)));
        }
    }
}
