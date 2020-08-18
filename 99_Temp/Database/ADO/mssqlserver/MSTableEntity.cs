using System.Text;
using DataBase.common;

namespace ServiceCore.Database.ADO.mssqlserver
{
    public abstract class MSTableEntity : TableEntity
    {
        public MSTableEntity()
            : base(null)
        {
        }
        public MSTableEntity(string table, DatabaseCore dba)
            : base(table, dba)
        {
        }
        protected override string CreateTableScript()
        {
            string script = null;
            if (this.ColumnCount > 0)
            {
                script = @"
                    CREATE TABLE [dbo].[{0}](
                        {1}
                        {2}
                    ) ON [PRIMARY]
                ";
                var pkscript = @"
                    ,CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                    (
	                    {1}
                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                    ";


                var columns = new StringBuilder();
                var pkeys = new StringBuilder();

                foreach (var column in this.GetColumns())
                {
                    if (columns.Length > 0) columns.Append(",");
                    columns.AppendLine(string.Format("[{0}] {1} {2}{3}{4}", 
                        column.ID, 
                        column.DBType, 
                        column.Nullable ? "NULL" : "NOT NULL", 
                        column.DefaultValue != null ? string.Format(" default('{0}')", column.DefaultValue.ToString()) : string.Empty,
                        column.KeyType == DataBase.common.enums.KeyType.IncrementPrimary ? " identity(1,1)" : string.Empty));

                    if (column.KeyType == DataBase.common.enums.KeyType.Primary
                        || column.KeyType == DataBase.common.enums.KeyType.IncrementPrimary)
                    {
                        if (pkeys.Length > 0) pkeys.Append(",");
                        pkeys.AppendLine(column.ID);
                    }
                }
                pkscript = string.Format(pkscript, this.TableName, pkeys.ToString());
                script = string.Format(script, this.TableName, columns.ToString(), pkeys.Length > 0 ? pkscript : string.Empty);
            }
            return script;
        }
    }
}
