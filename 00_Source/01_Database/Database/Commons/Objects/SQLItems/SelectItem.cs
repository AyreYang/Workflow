using Database.Entity;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Database.Interfaces;

namespace Database.Commons.Objects.SQLItems
{
    public class SelectItem : SQLItem
    {
        private const string SELECT = "SELECT";
        private const string DISTINCT = "DISTINCT";
        public bool Distinct { get; set; }
        private IList<TableItem> _tables { get; set; }
        internal TableItem[] Tables { get { return _tables.ToArray(); } }

        protected SelectItem(IDatabaseAccessor accessor, TableItem[] tables, bool distinct, Clause clause) : base(accessor, clause)
        {
            if (tables == null || tables.Count(t => t != null) <= 0) throw new ArgumentNullException("table");
            _tables = new List<TableItem>(tables);
            Distinct = distinct;
        }
        internal SelectItem(IDatabaseAccessor accessor, TableItem[] tables, bool distinct = true) : this(accessor, tables, distinct, null) { }
        public SelectItem(IDatabaseAccessor accessor, TableItem table, bool distinct = true) : this(accessor, new TableItem[] { table }, distinct) { }

        private string[] GetColumns()
        {
            var columns = new List<string>();
            foreach (var table in _tables)
            {
                columns.AddRange(table.Columns.Select(c => c.Value));
            }
            return columns.Distinct().ToArray();
        }

        protected override void BuildText(StringBuilder text)
        {
            text.AppendLine(Distinct ? string.Format("{0} {1}", SELECT, DISTINCT) : SELECT);
            text.Append(string.Join(",", GetColumns()));
        }

        public static SelectItem operator +(SelectItem item1, SelectItem item2)
        {
            if (item1 != null && item2 != null)
            {
                var tables = new List<TableItem>();
                tables.AddRange(item1.Tables);
                tables.AddRange(item2.Tables);
                return new SelectItem(item1._accessor, tables.ToArray());
            }
            else
            {
                if (item1 != null) return item1;
                if (item2 != null) return item2;
                return null;
            }
        }

        public override object Clone()
        {
            return new SelectItem(_accessor, Tables, Distinct, _clause);
        }
    }
}
