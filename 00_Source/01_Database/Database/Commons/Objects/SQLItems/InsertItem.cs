using Database.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects.SQLItems
{
    public class InsertItem : TableItem
    {
        private const string INSERT = "INSERT INTO";

        protected InsertItem(IDatabaseAccessor accessor, string table, Clause clause, params string[] columns) : base(accessor, table, null, clause, columns) { }

        public override object Clone()
        {
            return new InsertItem(this._accessor, _table, _clause, _columns.ToArray());
        }

        public InsertItem Append(Clause clause)
        {
            if (clause == null) return this;

            _clause = _clause == null ? clause : _clause.Append(clause);
            return this;
        }

        protected override void BuildText(StringBuilder text)
        {
            if (this._clause == null) throw new ApplicationException(string.Format("InsertItem({0}) is missing clause!", this.TableName));
            text.AppendLine(string.Format("{0} {1} ({2})", INSERT, this.TableName, string.Join(",", this.Columns.Select(c => c.Key))));
            text.Append(string.Format("VALUES ({0})", _clause.CommandText));
        }
    }
}
