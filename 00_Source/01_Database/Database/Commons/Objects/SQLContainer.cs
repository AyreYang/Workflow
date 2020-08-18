using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Commons.Objects
{
    public abstract class SQLContainer
    {
        public IDatabaseAccessor accessor { get; private set; }

        internal SQLParamCreater SQLParamCreater { get; private set; }
        protected Counter _tCounter { get; set; }
        protected Counter _pCounter { get; set; }
        public string SQLText
        {
            get
            {
                return BuildSQLText();
            }
        }
        public SQLContainer(IDatabaseAccessor accessor, Counter pcounter = null, Counter tcounter = null)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            this.accessor = accessor;

            _tCounter = tcounter == null ? new Counter() : tcounter;
            _pCounter = pcounter == null ? new Counter() : pcounter;
            SQLParamCreater = new SQLParamCreater(_pCounter);
        }

        public abstract DbCommand ExportCommand();
        protected abstract string BuildSQLText();

        public string NewTableAlias(string name = null)
        {
            return string.Format("{0}_{1}", string.IsNullOrWhiteSpace(name) ? "t" : name.Trim(), _tCounter.Count());
        }

        public SQLScriptParam NewParameter(object value)
        {
            return SQLParamCreater.NewParameter("param", value);
        }
        public SQLScriptParam NewParameter(string name, object value)
        {
            //var pnm = string.Format("{0}_{1}", (string.IsNullOrWhiteSpace(name) ? "param" : name.Trim()), _pCounter.Count());
            //return new SQLScriptParam(pnm, value);
            return SQLParamCreater.NewParameter(name, value);
        }
    }

    public class SQLParamCreater
    {
        private Counter _counter { get; set; }
        internal SQLParamCreater(Counter counter)
        {
            if (counter == null) throw new ArgumentNullException("counter");
            _counter = counter;
        }
        public SQLScriptParam NewParameter(object value)
        {
            return NewParameter("param", value);
        }
        public SQLScriptParam NewParameter(string name, object value)
        {
            var pnm = string.Format("{0}_{1}", (string.IsNullOrWhiteSpace(name) ? "param" : name.Trim()), _counter.Count());
            return new SQLScriptParam(pnm, value);
        }
    }

    public class SQLScriptParam
    {
        public string Name { get; private set; }
        public string CoveredName { get { return "{" + Name + "}"; } }
        public object Value { get; private set; }
        internal SQLScriptParam(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(name);

            Name = name.Trim();
            Value = value;
        }
        public KeyValuePair<string, object> ToKeyValuePair()
        {
            return new KeyValuePair<string, object>(Name, Value);
        }
    }
}
