using Database.Commons.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.Entity
{
    public class DBRefList<T> : DBRefBase
        where T : TableEntity
    {
        public override string Type => "DBRefList";

        public T[] Entities
        {
            get
            {
                ThrowNotReadyException();
                return _list.Select(i => i as T).ToArray();
            }
        }
        public bool IsEmpty { get { return _list.Count <= 0; } }

        public DBRefList() : base() { }
        public DBRefList(params T[] entities) : this()
        {
            Add(entities);
        }
        public DBRefList(IList<T> entities) : this(entities == null ? new T[0] : entities.ToArray()) { }

        public bool Any(Func<T, bool> predicate)
        {
            ThrowNotReadyException();

            return _list.Any(i => predicate(i as T));
        }

        public void Add(params T[] entities)
        {
            ThrowNotReadyException();

            if (entities != null && entities.Length > 0)
            {
                _list.AddRange(entities);
            }
        }

        public void Delete(Predicate<T> match)
        {
            ThrowNotReadyException();

            var entities = _list.FindAll(i => match(i as T));
            if (entities != null && entities.Count > 0)
            {
                entities.ForEach(ent =>
                {
                    _list0.Add(ent);
                    _list.Remove(ent);
                });
            }
        }

        public T[] Find(Predicate<T> match)
        {
            ThrowNotReadyException();
            return _list.FindAll(i => match(i as T)).Select(i => i as T).ToArray();
        }

        public T FirstOrDefault(Func<T, bool> predicate = null)
        {
            ThrowNotReadyException();

            return predicate == null ?
                _list.Count > 0 ? _list[0] as T : null
                : _list.FirstOrDefault(i => predicate(i as T)) as T;
        }

        public void Sort(Comparison<T> comparison)
        {
            ThrowNotReadyException();

            _list.Sort((e1, e2) => comparison(e1 as T, e2 as T));
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
            ThrowNotReadyException();

            if (collection == null) throw new ArgumentNullException("collection");
            _list0.ForEach(item => {
                item.Schema.FillDeleteScript(collection);
            });
            _list.ForEach(ent =>
            {
                ent.Schema.FillSaveScript(collection);
            });
        }

        internal override void Delete(SQLScriptCollection collection)
        {
            ThrowNotReadyException();

            if (collection == null) throw new ArgumentNullException("collection");
            _list0.ForEach(item => {
                item.Schema.FillDeleteScript(collection);
            });
            _list.ForEach(ent =>
            {
                ent.Schema.FillDeleteScript(collection);
            });
        }

    }
}
