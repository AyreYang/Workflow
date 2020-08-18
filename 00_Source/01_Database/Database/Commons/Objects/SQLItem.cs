using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Commons.Objects
{
    public abstract class SQLItem : ICloneable
    {
        internal IDatabaseAccessor _accessor { get; private set; }
        protected Clause _clause{ get; set; }
        public KeyValuePair<string, object>[] Parameters
        {
            get { return _clause != null ? _clause.CommandParameters : null; }
        }

        public string Text
        {
            get 
            {
                return BuildText();
            }
        }

        public SQLItem(IDatabaseAccessor accessor) : this(accessor, null) { }

        protected SQLItem(IDatabaseAccessor accessor, Clause clause)
        {
            _accessor = accessor;
            _clause = clause != null ? clause.Clone() : null;
        }

        private string BuildText()
        {
            var buffer = new StringBuilder();
            BuildText(buffer);
            return buffer.ToString();
        }
        protected abstract void BuildText(StringBuilder text);

        public abstract object Clone();
    }
}
