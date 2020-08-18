using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects.SQLItems
{
    public class JoinItem : TableItem
    {
        private int[] FLAGS = new int[]{0,1,2};
        private const string JOIN0 = "INNER";
        private const string JOIN1 = "LEFT";
        private const string JOIN2 = "RIGHT";

        protected int _flag { get; private set; }
        private string Join 
        {
            get
            {
                string join = null;
                switch (_flag)
                {
                    case 0:
                        join = JOIN0;
                        break;
                    case 1:
                        join = JOIN1;
                        break;
                    case 2:
                        join = JOIN2;
                        break;
                }
                return join;
            }
        }

        protected JoinItem(IDatabaseAccessor accessor, string table, string alias, int flag, Clause clause, params string[] columns) : base(accessor, table, alias, clause, columns)
        {
            if (!FLAGS.Any(f => f.Equals(flag))) throw new ApplicationException(string.Format("JoinItem(flag:{0}) is illegal!(0:INNER,1:LEFT,2:RIGHT)", flag));
        }
        public JoinItem(IDatabaseAccessor accessor, string table, string alias, int flag, params string[] columns) : this(accessor, table, alias, flag, null, columns) { }
        public JoinItem(IDatabaseAccessor accessor, string table, params string[] columns) : this(accessor, table, null, 0, columns) { }

        public JoinItem ON(TableItem table, params KeyValuePair<string, string>[] keys)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (keys == null || keys.Length <= 0) throw new ArgumentNullException("keys");
            foreach(var pair in keys)
            {
                if (!this.Columns.Any(c => c.Key.Equals(pair.Key.ToUpper()))) throw new ApplicationException(string.Format("Key1({0}) is not in table({1})", pair.Key, this.TableName));
                if (!table.Columns.Any(c => c.Key.Equals(pair.Value.ToUpper()))) throw new ApplicationException(string.Format("Key2({0}) is not in table({1})", pair.Value, table.TableName));
                var key1 = this.Columns.First(c => c.Key.Equals(pair.Key.ToUpper())).Value;
                var key2 = table.Columns.First(c => c.Key.Equals(pair.Value.ToUpper())).Value;
                var clause = new Clause(_accessor, string.Format("{0} = {1}", key1, key2));
                //_clause = _clause == null ? clause : _clause * clause;
                ON(clause);
            }
            return this;
        }
        public JoinItem ON(string column, SQLScriptParam param)
        {
            if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            if (param == null) throw new ArgumentNullException("param");
            if (!this.Columns.Any(c => c.Key.Equals(column.Trim().ToUpper()))) throw new ApplicationException(string.Format("Column({0}) is not in table({1})", column, this.TableName));
            var key = this.Columns.First(c => c.Key.Equals(column.Trim().ToUpper())).Value;
            var clause = new Clause(_accessor, string.Format("{0} = {1}", key, param.CoveredName), new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
            return ON(clause);
        }
        public JoinItem ON(Clause clause)
        {
            if(clause != null)
            {
                _clause = _clause == null ? clause : _clause * clause;
            }
            return this;
        }
        protected override void BuildText(StringBuilder text)
        {
            if (_clause == null) throw new ApplicationException(string.Format("JoinItem({0}) is missing clause!", this.TableName));
            text.Append(string.Format("{0} JOIN {1} ON {2}", Join, TableName, _clause.CommandText));
        }


        public override object Clone()
        {
            return new JoinItem(_accessor, _table, _alias, _flag, _clause, _columns.ToArray());
        }
    }
}
