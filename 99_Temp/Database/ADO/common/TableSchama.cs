using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataBase.common;
using DataBase.common.interfaces;
using System.Data.Common;
using DataBase.common.enums;

namespace ServiceCore.Database.ADO.common
{
    public class TableSchama
    {
        private const string SQL_SELECT = @"SELECT * FROM [{0}]";
        private const string SQL_TRUNCATE = @"TRUNCATE TABLE [{0}]";
        private const string SQL_DROP = @"DROP TABLE [{0}]";
        private const string SQL_CREATE = @"CREATE TABLE [dbo].[{0}](
                                                {1}
                                                {2}
                                            ) ON [PRIMARY]
                                        ";
        private const string SQL_CONSTRAINT_PK = @",CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    {1}
                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                    ";

        private const string SQL_INSERT_1 = @"INSERT INTO [{0}] VALUES({1})";

        public bool Ready { get; private set; }
        public TableSchamaType TYPE { get; private set; }
        public string TableName { get; private set; }
        public string SchamaSQL { get; private set; }
        private List<SchamaItem> _schama { get; set; }
        public IDatabase DBAccessor { get; private set; }

        public bool Exists
        {
            get
            {
                return TableName != null ? DBAccessor.TableExists(TableName) : false;
            }
        }

        public TableSchama(string table, IDatabase dba)
        {
            if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException("parameter(table) is null or empty!");
            if (dba == null) throw new ArgumentNullException("parameter(dba) is null!");

            TYPE = TableSchamaType.TABLE;

            _schama = new List<SchamaItem>();
            TableName = table.Trim();
            DBAccessor = dba;
            SchamaSQL = string.Format(SQL_SELECT, TableName);

            LoadSchama();
        }

        public TableSchama(IDatabase dba, string sql)
        {
            
            if (dba == null) throw new ArgumentNullException("parameter(dba) is null!");
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException("parameter(sql) is null or empty!");

            TYPE = TableSchamaType.VIEW;

            _schama = new List<SchamaItem>();
            TableName = null;
            DBAccessor = dba;
            SchamaSQL = sql.Trim();

            LoadSchama();
        }

        public void Fresh()
        {
            LoadSchama();
        }

        private void LoadSchama()
        {
            Ready = false;
            if ((TYPE == TableSchamaType.TABLE && Exists) || (TYPE == TableSchamaType.VIEW))
            {
                DBAccessor.RetrieveReader(DBAccessor.CreateCommand(SchamaSQL), (reader) =>
                {
                    var schamatable = reader.GetSchemaTable();
                    if (schamatable != null && schamatable.Rows.Count > 0)
                    {
                        _schama.Clear();
                        foreach (DataRow row in schamatable.Rows)
                        {
                            var sitm = new SchamaItem(row);
                            sitm.SetColumnType(reader.GetFieldType(sitm.ColumnOrdinal));
                            _schama.Add(sitm);
                        }
                    }
                });
            }
            Ready = _schama.Count > 0;
        }

        public DbCommand GetTruncateCommand()
        {
            return TYPE == TableSchamaType.TABLE ? DBAccessor.CreateCommand(string.Format(SQL_TRUNCATE, TableName)) : null;
        }
        public DbCommand GetSelectCommand()
        {
            return DBAccessor.CreateCommand(SchamaSQL);
        }
        public DbCommand GetCreateCommand(string newtable = null)
        {
            DbCommand command = null;
            var name = newtable == null ? TableName : newtable.Trim();
            if (string.IsNullOrWhiteSpace(name)) throw new ApplicationException("table name is empty!");
            string script = null;
            if (_schama.Count > 0)
            {
                var columns = new StringBuilder();
                var pkeys = new StringBuilder();

                foreach (var item in _schama)
                {
                    if (columns.Length > 0) columns.Append(",");
                    columns.AppendLine(string.Format("[{0}] {1} {2}{3}{4}",
                        item.ColumnName,
                        item.DataType,
                        item.AllowDBNull ? "NULL" : "NOT NULL",
                        item.DefaultValue != null ? string.Format(" default('{0}')", item.DefaultValue.ToString()) : string.Empty,
                        item.IsAutoIncrement ? " identity(1,1)" : string.Empty));

                    if (item.IsPrimaryKey)
                    {
                        if (pkeys.Length > 0) pkeys.Append(",");
                        pkeys.AppendLine(item.ColumnName);
                    }
                }
                var pkscript = string.Format(SQL_CONSTRAINT_PK, name, pkeys.ToString());
                script = string.Format(SQL_CREATE, name, columns.ToString(), pkeys.Length > 0 ? pkscript : string.Empty);
                command = DBAccessor.CreateCommand(script);
            }
            return command;
        }
        public DbCommand GetInsertCommand(object[] parameters)
        {
            if (TYPE != TableSchamaType.TABLE) return null;
            if (_schama.Count <= 0) return null;

            var sb_param = new StringBuilder();
            var lst_params = new List<DbParameter>();
            var items = _schama.OrderBy(itm => itm.ColumnOrdinal);
            foreach (var item in items)
            {
                if (item.ColumnOrdinal >= parameters.Length) break;

                if (sb_param.Length > 0) sb_param.Append(",");
                var parameter = DBAccessor.CreateParameter(string.Format("COL{0}", item.ColumnOrdinal), parameters[item.ColumnOrdinal]);
                sb_param.Append(parameter.ParameterName);
                lst_params.Add(parameter);
            }
            var sql = string.Format(SQL_INSERT_1, TableName, sb_param.ToString());
            return DBAccessor.CreateCommand(sql, lst_params.ToArray());
        }

        public void SetSchama(string column, int? size, short? precision, short? scale, bool? ispk, bool? nullable, bool? increment)
        {
            if (string.IsNullOrWhiteSpace(column)) throw new ArgumentNullException("parameter(column) is null or empty!");
            var colnm = column.Trim().ToUpper();
            var schamaitem = _schama.FirstOrDefault(i => i.ColumnName.Equals(colnm));
            if (schamaitem == null) throw new ApplicationException(string.Format("column({0}) does not exist!", column));

            if (size.HasValue) schamaitem.SetColumnSize(size.Value);
            if (precision.HasValue) schamaitem.SetPrecision(precision.Value);
            if (scale.HasValue) schamaitem.SetScale(scale.Value);
            if (ispk.HasValue) schamaitem.SetPrimaryKey(ispk.Value);
            if (nullable.HasValue) schamaitem.SetNullable(nullable.Value);
            if (increment.HasValue) schamaitem.SetIncrement(increment.Value);
        }
        public void SetSchama(string column, int? size, short? precision, short? scale, bool? ispk, bool? nullable, bool? increment, object defval)
        {
            if(string.IsNullOrWhiteSpace(column))throw new ArgumentNullException("parameter(column) is null or empty!");
            var colnm = column.Trim().ToUpper();
            var schamaitem = _schama.FirstOrDefault(i => i.ColumnName.Equals(colnm));
            if (schamaitem == null) throw new ApplicationException(string.Format("column({0}) does not exist!", column));

            if (size.HasValue) schamaitem.SetColumnSize(size.Value);
            if (precision.HasValue) schamaitem.SetPrecision(precision.Value);
            if (scale.HasValue) schamaitem.SetScale(scale.Value);
            if (ispk.HasValue) schamaitem.SetPrimaryKey(ispk.Value);
            if (nullable.HasValue) schamaitem.SetNullable(nullable.Value);
            if (increment.HasValue) schamaitem.SetIncrement(increment.Value);

            schamaitem.SetDefaultValue(defval);
        }
        public void SetSchama(string column, int? size, short? precision, short? scale)
        {
            SetSchama(column, size, precision, scale, null, null, null);
        }
        public void SetSchama(string column, bool? ispk, bool? nullable, bool? increment)
        {
            SetSchama(column, null, null, null, ispk, nullable, increment);
        }
        public void SetSchama(string column, object defval)
        {
            SetSchama(column, null, null, null, null, null, null, defval);
        }

        public long TruncateTable()
        {
            long rows = 0;
            if (TYPE == TableSchamaType.TABLE && Exists)
            {
                var command = GetTruncateCommand();
                rows = DBAccessor.ExecuteSQLCommand(command);
            }
            return rows;
        }
        public bool CreateTableBySchama(TableSchama source)
        {
            if (source == null) throw new ArgumentNullException("parameter(source) is null!", "source");
            bool result = false;
            var command = source.GetCreateCommand(this.TableName);
            if (command != null)
            {
                result = DBAccessor.ExecuteSQLCommand(command) >= 0;
            }
            Fresh();
            return result;
        }

        public void RetrieveReader(Action<DbDataReader> action)
        {
            if (action == null) throw new ArgumentNullException("parameter(action) is null!", "action");
            var command = GetSelectCommand();
            DBAccessor.RetrieveReader(command, action);
        }
        public long ImportFrom(TableSchama schama, bool truncate)
        {
            var rows = DBAccessor.ImportTable(schama, this, truncate);
            return rows >= 0 ? rows : -4;
        }
    }

    internal class SchamaItem
    {
        private const string PATTERN0 = "[{0}]";
        private const string PATTERN1 = "[{0}]({1})";
        private const string PATTERN2 = "[{0}]({1},{2})";

        public int ColumnOrdinal { get; private set; }
        public string ColumnName { get; private set; }
        public string DataTypeName { get; private set; }
        public string DataType
        {
            get
            {
                return Convert2DataType();
            }
        }
        public int ColumnSize { get; private set; }
        public short NumericPrecision { get; private set; }
        public short NumericScale { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public bool AllowDBNull { get; private set; }
        public bool IsAutoIncrement { get; private set; }
        public object DefaultValue { get; private set; }
        public Type Type { get; private set; }

        internal SchamaItem(DataRow row)
        {
            ColumnOrdinal = (int)row["ColumnOrdinal"];
            ColumnName = row["ColumnName"].ToString().Trim().ToUpper();
            DataTypeName = row["DataTypeName"].ToString().Trim();

            ColumnSize = (int)row["ColumnSize"];
            NumericPrecision = (short)row["NumericPrecision"];
            NumericScale = (short)row["NumericScale"];

            IsPrimaryKey = row["IsKey"] != DBNull.Value ? (bool)row["IsKey"] : false;
            AllowDBNull = row["AllowDBNull"] != DBNull.Value ? (bool)row["AllowDBNull"] : false;
            IsAutoIncrement = row["IsAutoIncrement"] != DBNull.Value ? (bool)row["IsAutoIncrement"] : false;

            DefaultValue = null;

            Type = null;
        }

        internal void SetColumnSize(int size)
        {
            if (size >= 0) ColumnSize = size;
        }
        internal void SetPrecision(short precision)
        {
            if (precision >= 0) NumericPrecision = precision;
        }
        internal void SetScale(short scale)
        {
            if (scale >= 0) NumericScale = scale;
        }
        internal void SetPrimaryKey(bool flag)
        {
            IsPrimaryKey = flag;
        }
        internal void SetNullable(bool flag)
        {
            AllowDBNull = flag;
        }
        internal void SetIncrement(bool flag)
        {
            IsAutoIncrement = flag;
        }
        internal void SetDefaultValue(object defval)
        {
            DefaultValue = defval;
        }
        internal void SetColumnType(Type type)
        {
            if (type != null) this.Type = type;
        }

        private string Convert2DataType()
        {
            string type = null;
            switch (DataTypeName)
            {
                case "uniqueidentifier":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "char":
                    type = string.Format(PATTERN1, DataTypeName, ColumnSize);
                    break;
                case "nchar":
                    type = string.Format(PATTERN1, DataTypeName, ColumnSize);
                    break;
                case "varchar":
                    type = string.Format(PATTERN1, DataTypeName, ColumnSize);
                    break;
                case "nvarchar":
                    type = string.Format(PATTERN1, DataTypeName, ColumnSize);
                    break;
                case "text":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "ntext":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "int":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "bigint":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "smallint":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "tinyint":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "float":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "decimal":
                    type = string.Format(PATTERN2, DataTypeName, NumericPrecision, NumericScale);
                    break;
                case "numeric":
                    type = string.Format(PATTERN2, DataTypeName, NumericPrecision, NumericScale);
                    break;
                case "money":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "smallmoney":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "bit":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "binary":
                    type = string.Format(PATTERN1, DataTypeName, ColumnSize);
                    break;
                case "image":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "date":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "time":
                    type = string.Format(PATTERN1, DataTypeName, ColumnSize);
                    break;
                case "datetime":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
                case "timestamp":
                    type = string.Format(PATTERN0, DataTypeName);
                    break;
            }
            return type;
        }
    }
}
