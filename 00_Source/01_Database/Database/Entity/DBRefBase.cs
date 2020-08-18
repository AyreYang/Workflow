using Database.Commons.Objects;
using Database.Entity.Enums;
using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entity
{
    public abstract class DBRefBase : IDisposable
    {
        public bool Ready { get; private set; }
        protected List<TableEntity> _list { get; private set; }
        //internal TableEntity[] List
        //{
        //    get
        //    {
        //        return _list.ToArray();
        //    }
        //}
        protected List<TableEntity> _list0 { get; private set; }

        public STATUS Status
        {
            get
            {
                //STATUS st = STATUS.RAW;
                //if(_list.Count > 0)
                //{
                //    _list.ForEach(e =>
                //    {
                //        st = (STATUS)Math.Max((int)st, (int)e.Schema.Status);
                //    });
                //}
                //var max = _list.Max(e => e.Schema.Status);


                return _list0.Count > 0 ? STATUS.CHANGED :
                    _list.Count > 0 ?
                    _list.Max(e => e.Schema.Status) : STATUS.RAW;
            }
        }

        public abstract string Type { get; }
        public abstract TableEntity NewTableEntity();
        internal abstract void Save(SQLScriptCollection collection);
        internal abstract void Delete(SQLScriptCollection collection);

        public void Save(IDatabaseAccessor accessor)
        {
            ThrowNotReadyException();

            if (accessor == null) throw new ArgumentNullException("accessor");
            var collection = new SQLScriptCollection(accessor);
            Save(collection);
            accessor.ExecuteSQLCommand(collection.ExportCommands());
        }
        public void Delete(IDatabaseAccessor accessor)
        {
            ThrowNotReadyException();

            if (accessor == null) throw new ArgumentNullException("accessor");
            var collection = new SQLScriptCollection(accessor);
            Delete(collection);
            accessor.ExecuteSQLCommand(collection.ExportCommands());
        }

        protected void ThrowNotReadyException()
        {
            if (!Ready) throw new ApplicationException("Not ready now!");
        }

        public DBRefBase()
        {
            _list = new List<TableEntity>();
            _list0 = new List<TableEntity>();

            Ready = true;
        }

        internal void Clear()
        {
            ThrowNotReadyException();
            _list.Clear();
            _list0.Clear();
        }

        public void Dispose()
        {
            Ready = false;

            if (_list != null) _list.Clear();
            if (_list0 != null) _list0.Clear();

            _list = null;
            _list0 = null;
        }
    }
}
