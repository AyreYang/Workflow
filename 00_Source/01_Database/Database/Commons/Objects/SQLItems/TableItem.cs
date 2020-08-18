using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Database.Commons.Objects.SQLItems
{
    public abstract class TableItem : SQLItem
    {
        protected string _table { get; private set; }
        public string TableName
        {
            get { return string.IsNullOrWhiteSpace(Alias) ? _table : string.Format("{0} {1}", _table, _alias); }
        }
        public string NativeName
        {
            get
            {
                return _table;
            }
        }
        protected string _alias { get; private set; }
        public string Alias
        {
            get { return _alias; }
            set
            {
                CheckTableAlias(value);
                _alias = value;
            }
        }
        protected IList<string> _columns { get; private set; }
        public KeyValuePair<string, string>[] Columns
        {
            get
            {
                return _columns.Select(c =>
                new KeyValuePair<string, string>(c, string.Format("{0}.{1}", string.IsNullOrWhiteSpace(Alias) ? _table : _alias, c))
                ).ToArray();
            }
        }
        protected TableItem(IDatabaseAccessor accessor, string table, string alias, Clause clause, params string[] columns) : base(accessor, clause)
        {
            CheckTableName(table);
            _table = table;

            CheckColumnNames(columns);
            _columns = new List<string>(columns.Select(c => c.ToUpper()).Distinct());

            Alias = alias;
        }
        public TableItem(IDatabaseAccessor accessor, string table, string alias, params string[] columns) : this(accessor, table, alias, null, columns) { }
        public TableItem(IDatabaseAccessor accessor, string table, params string[] columns) : this(accessor, table, null, columns) { }

        public string ConvertColumn(string column)
        {
            if (string.IsNullOrWhiteSpace(column)) return null;
            if (!this.Columns.Any(c => c.Key.Equals(column.Trim().ToUpper()))) return null;
            return this.Columns.First(c => c.Key.Equals(column.Trim().ToUpper())).Value;
        }
        
        #region Create Clause
        public Clause ColumnIsNull(string column)
        {
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} IS NULL", ConvertColumn(column)));
        }
        public Clause ColumnIsNotNull(string column)
        {
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} IS NOT NULL", ConvertColumn(column)));
        }

        public Clause ColumnEquals(string column, SQLScriptParam param)
        {
            if (param == null) throw new ArgumentNullException("param");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} = {1}", ConvertColumn(column), param.CoveredName), 
                new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnNotEquals(string column, SQLScriptParam param)
        {
            if (param == null) throw new ArgumentNullException("param");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} <> {1}", ConvertColumn(column), param.CoveredName),
                new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnGreater(string column, SQLScriptParam param, bool equals = false)
        {
            if (param == null) throw new ArgumentNullException("param");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format(equals ? "{0} >= {1}" : "{0} > {1}", ConvertColumn(column), param.CoveredName),
                new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnLess(string column, SQLScriptParam param, bool equals = false)
        {
            if (param == null) throw new ArgumentNullException("param");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format(equals ? "{0} <= {1}" : "{0} < {1}", ConvertColumn(column), param.CoveredName),
                new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnLike(string column, SQLScriptParam param)
        {
            if (param == null) throw new ArgumentNullException("param");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} LIKE {1}", ConvertColumn(column), param.CoveredName),
                new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }

        public Clause ColumnNotLike(string column, SQLScriptParam param)
        {
            if (param == null) throw new ArgumentNullException("param");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} NOT LIKE {1}", ConvertColumn(column), param.CoveredName),
                new KeyValuePair<string, object>[] { param.ToKeyValuePair() });
        }
        public Clause ColumnBetween(string column, SQLScriptParam param1, SQLScriptParam param2)
        {
            if (param1 == null) throw new ArgumentNullException("param1");
            if (param2 == null) throw new ArgumentNullException("param2");
            CheckColumnName(column);
            return new Clause(_accessor, string.Format("{0} BETWEEN {1} AND {2}", ConvertColumn(column), param1.CoveredName, param2.CoveredName),
                new KeyValuePair<string, object>[] { param1.ToKeyValuePair(), param2.ToKeyValuePair() });
        }
        #endregion
        
        private void CheckTableName(string name)
        {
            if(!CheckName(name)) throw new ApplicationException(string.Format("TableName({0}) is illegal!", name));
        }
        private void CheckTableAlias(string name)
        {
            if (name != null && !CheckName(name)) throw new ApplicationException(string.Format("TableAlias({0}) is illegal!", name));
        }
        private void CheckColumnNames(string[] columns)
        {
            if (columns == null || columns.Length <= 0) throw new ApplicationException("Table contains no column!");
            foreach (var column in columns)
            {
                if (!CheckName(column)) throw new ApplicationException(string.Format("TableColumn({0}) is illegal!", column));
            }
        }
        private void CheckColumnName(string column)
        {
            if (string.IsNullOrWhiteSpace(column)) throw new ApplicationException("Column name is null or empty!");
            if(!this.Columns.Any(c => c.Key.Equals(column.Trim().ToUpper()))) throw new ApplicationException(string.Format("Column({0}) unexists in table({1})!", column, TableName));
        }

        private bool CheckName(string name)
        {
            return Regex.IsMatch(name ?? string.Empty, @"^[_a-zA-Z][_a-zA-Z0-9]+$");
        }
    }
}
