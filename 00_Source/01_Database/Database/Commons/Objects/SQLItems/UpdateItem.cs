using Database.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects.SQLItems
{
    public class UpdateItem : TableItem
    {
        private const string UPDATE = "UPDATE";

        protected UpdateItem(IDatabaseAccessor accessor, string table, Clause clause, params string[] columns) : base(accessor, table, null, clause, columns) { }

        public override object Clone()
        {
            return new UpdateItem(this._accessor, _table, _clause, _columns.ToArray());
        }

        public UpdateItem Append(Clause clause)
        {
            if (clause == null) return this;

            _clause = _clause == null ? clause : _clause.Append(clause);
            return this;
        }

        protected override void BuildText(StringBuilder text)
        {
            if (this._clause == null) throw new ApplicationException(string.Format("UpdateItem({0}) is missing clause!", this.TableName));
            text.AppendLine(string.Format("{0} {1}", UPDATE, this.TableName));
            text.Append(string.Format("SET {0}", _clause.CommandText));
        }
    }
}
