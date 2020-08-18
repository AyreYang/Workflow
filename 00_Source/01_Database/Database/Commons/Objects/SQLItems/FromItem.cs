using Database.Interfaces;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects.SQLItems
{
    public class FromItem : TableItem
    {
        private const string FROM = "FROM";

        protected FromItem(IDatabaseAccessor accessor, string table, string alias, Clause clause, params string[] columns) : base(accessor, table, alias, clause, columns) { }
        public FromItem(IDatabaseAccessor accessor, string table, string alias, params string[] columns) : this(accessor, table, alias, null, columns) { }
        public FromItem(IDatabaseAccessor accessor, string table, params string[] columns) : this(accessor, table, null, columns) { }
        protected override void BuildText(StringBuilder text)
        {
            text.Append(string.Format("{0} {1}", FROM, this.TableName));
        }

        public override object Clone()
        {
            return new FromItem(_accessor, _table, _alias, _clause, _columns.ToArray());
        }
    }
}
