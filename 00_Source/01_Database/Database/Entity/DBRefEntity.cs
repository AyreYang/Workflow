using System;
using System.Linq;
using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Entity.SQLScripts.SQLItems;
using Database.Interfaces;

namespace Database.Entity
{
    public class DBRefEntity<T> : DBRefBase
        where T : TableEntity
    {
        public override string Type => "DBRefEntity";

        public DBRefEntity(T entity = null) : base()
        {
            if(entity != null) _list.Add(entity);
        }

        public T Entity
        {
            get
            {
                ThrowNotReadyException();
                return (_list.Count > 0) ? _list.First() as T : null;
            }
            set
            {
                ThrowNotReadyException();
                _list.Clear();
                if (value != null)
                {
                    _list.Add(value);
                }
            }
        }

        public void Delete()
        {
            ThrowNotReadyException();

            if (_list.Count > 0 && _list.First().Schema.Status >= Enums.STATUS.FRESHED) _list0.Add(_list.First());
            _list.Clear();
        }

        public override TableEntity NewTableEntity()
        {
            return NewEntity();
        }

        public T NewEntity()
        {
            ThrowNotReadyException();

            var entity = System.Activator.CreateInstance(typeof(T)) as T;
            _list.Add(entity);
            return entity;
        }

        internal override void Save(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            _list0.ForEach(item =>{
                item.Schema.FillDeleteScript(collection);
            });
            if(Entity != null) Entity.Schema.FillSaveScript(collection);
        }

        internal override void Delete(SQLScriptCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            _list0.ForEach(item => {
                item.Schema.FillDeleteScript(collection);
            });
            if (Entity != null) Entity.Schema.FillDeleteScript(collection);
        }
    }
}
