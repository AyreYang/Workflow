using Database.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects.SQLItems
{
    public class DeleteItem : TableItem
    {
        private const string DELETE = "DELETE";

        protected DeleteItem(IDatabaseAccessor accessor, string table, Clause clause, params string[] columns) : base(accessor, table, null, clause, columns) { }

        public override object Clone()
        {
            return new DeleteItem(this._accessor, _table, _clause, _columns.ToArray());
        }

        protected override void BuildText(StringBuilder text)
        {
            text.AppendLine(string.Format("{0} FROM {1}", DELETE, this.TableName));
        }
    }
}
