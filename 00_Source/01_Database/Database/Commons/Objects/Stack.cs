using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Commons.Objects
{
    public class TStack<T>
    {
        private IList<T> _list { get; set; }
        public int Count
        {
            get { return _list.Count; }
        }
        public bool IsEmpty
        {
            get { return Count <= 0; }
        }

        public TStack()
        {
            _list = new List<T>();
        }
        internal TStack(IList<T> list)
        {
            _list = list != null ? new List<T>(list) : new List<T>();
        }

        public void Add(T item)
        {
            _list.Insert(0, item);
        }
        public T Fetch()
        {
            if (IsEmpty)
            {
                throw new ApplicationException("Stack is empty!");
            }
            var item = _list.First();
            _list.RemoveAt(0);
            return item;
        }
        public void Clear()
        {
            _list.Clear();
        }

        public Stack<T> Copy()
        {
            return new Stack<T>(this._list);
        }
    }
}
