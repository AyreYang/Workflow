using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Commons.Objects.SQLItems
{
    public class WhereItem : SQLItem
    {
        private const string WHERE = "WHERE";

        protected TableItem _table { get; private set; }
        public TableItem TableItem { get { return _table; } }

        internal Clause Clause
        {
            get { return _clause; }
        }

        public WhereItem(IDatabaseAccessor accessor, Clause clause, TableItem table) : base(accessor, clause) 
        {
            _table = table;
        }
        public WhereItem(IDatabaseAccessor accessor, Clause clause) : this(accessor, clause, null) { }
        public WhereItem(IDatabaseAccessor accessor, TableItem table) : this(accessor, null, table) { }

        private void CheckTableBind()
        {
            if (_table == null) throw new ApplicationException("The WhereItem doesn`t bind to table!");
        }
        public Clause ColumnIsNull(string column)
        {
            CheckTableBind();
            return _table.ColumnIsNull(column);

            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if(col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} IS NULL", col));
        }
        public Clause ColumnIsNotNull(string column)
        {
            CheckTableBind();
            return _table.ColumnIsNotNull(column);

            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} IS NOT NULL", col));
        }
        public Clause ColumnEquals(string column, SQLScriptParam param)
        {
            CheckTableBind();
            return _table.ColumnEquals(column, param);
            //if (param == null) throw new ArgumentNullException("param");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} = {1}", col, param.CoveredName),
            //    new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnNotEquals(string column, SQLScriptParam param)
        {
            CheckTableBind();
            return _table.ColumnNotEquals(column, param);
            //if (param == null) throw new ArgumentNullException("param");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} <> {1}", col, param.CoveredName),
            //    new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnGreater(string column, SQLScriptParam param, bool equals = false)
        {
            CheckTableBind();
            return _table.ColumnGreater(column, param, equals);

            //if (param == null) throw new ArgumentNullException("param");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format(equals ? "{0} >= {1}" : "{0} > {1}", col, param.CoveredName),
            //    new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnLess(string column, SQLScriptParam param, bool equals = false)
        {
            CheckTableBind();
            return _table.ColumnLess(column, param, equals);

            //if (param == null) throw new ArgumentNullException("param");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format(equals ? "{0} <= {1}" : "{0} < {1}", col, param.CoveredName),
            //    new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnLike(string column, SQLScriptParam param)
        {
            CheckTableBind();
            return _table.ColumnLike(column, param);

            //if (param == null) throw new ArgumentNullException("param");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} LIKE {1}", col, param.CoveredName),
            //    new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }

        public Clause ColumnNotLike(string column, SQLScriptParam param)
        {
            CheckTableBind();
            return _table.ColumnNotLike(column, param);

            //if (param == null) throw new ArgumentNullException("param");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} NOT LIKE {1}", col, param.CoveredName),
            //    new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnBetween(string column, SQLScriptParam param1, SQLScriptParam param2)
        {
            CheckTableBind();
            return _table.ColumnBetween(column, param1, param2);

            //if (param1 == null) throw new ArgumentNullException("param1");
            //if (param2 == null) throw new ArgumentNullException("param2");
            //if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("column");
            //var col = _table != null ? _table.ConvertColumn(column) : column.Trim();
            //if (col == null) col = column.Trim();

            //return new Clause(_accessor, string.Format("{0} BETWEEN {1} AND {2}", col, param1.CoveredName, param2.CoveredName),
            //    new KeyValuePair<string, object>[] { param1.ToKeyValuePair(), param2.ToKeyValuePair() });
        }


        public WhereItem And(Clause clause)
        {
            if (clause == null) return this;

            _clause = clause == null ? clause : _clause * clause;
            return this;
        }
        public WhereItem Or(Clause clause)
        {
            if (clause == null) return this;

            _clause = clause == null ? clause : _clause + clause;
            return this;
        }

        protected override void BuildText(StringBuilder text)
        {
            if (_clause != null)
            {
                text.Append(string.Format("{0} {1}", WHERE, _clause.CommandText));
            }
        }

        public override object Clone()
        {
            return new WhereItem(_accessor, _clause, _table);
        }

        public static WhereItem operator +(WhereItem item1, WhereItem item2)
        {
            if (item1 != null && item2 != null)
            {
                return new WhereItem(item1._accessor, item1.Clause * item2.Clause);
            }
            else
            {
                if (item1 != null) return item1;
                if (item2 != null) return item2;
                return null;
            }
        }
    }
}
