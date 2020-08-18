using Database.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Implements.SQLite
{
    sealed public class DatabaseAccessor : DatabaseCore
    {
        private const string PREFIX_PARM = "@";
        public const int COMMAND_TIMEOUT = 600;

        public DatabaseAccessor(string dbfile)
        {
            mcnt_info = new ConnectionInfo(dbfile);
            if (!System.IO.File.Exists(dbfile))
            {
                SQLiteConnection.CreateFile(dbfile);
            }
        }

        protected override DbConnection CreateConnection(string cnnstr)
        {
            return new SQLiteConnection(cnnstr);
        }

        //public override bool TableExists(string as_table)
        //{
        //    if (string.IsNullOrWhiteSpace(as_table)) return false;
        //    var count = CountInTable("sqlite_master", new Clause("type='table' and name = {name}").AddParam("name", as_table.Trim()));
        //    return count > 0;
        //}

        public override string CreateParameterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return (!name.Trim().StartsWith(PREFIX_PARM)) ? PREFIX_PARM + name.Trim().ToUpper() : name.Trim().ToUpper();
        }

        private object ConvertValue(object value)
        {
            if (value == null) return DBNull.Value;
            if (value is Guid) return value.ToString();
            return value;
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            var pname = CreateParameterName(name);
            if (string.IsNullOrWhiteSpace(pname)) return null;
            //return new SQLiteParameter(pname, value == null ? DBNull.Value : value);
            return new SQLiteParameter(pname, ConvertValue(value));
        }
        public override DbParameter CreateParameter(string name, DbType type, int size, object value, ParameterDirection direction)
        {
            var pname = CreateParameterName(name);
            if (string.IsNullOrWhiteSpace(pname)) return null;
            DbParameter parameter = new SQLiteParameter(pname, type, size);
            //parameter.Value = value == null ? DBNull.Value : value;
            parameter.Value = ConvertValue(value);
            parameter.Direction = direction;
            return parameter;
        }
        public override DbCommand CreateCommand(string sql, params DbParameter[] parameters)
        {
            var command = new SQLiteCommand(sql);
            command.CommandTimeout = COMMAND_TIMEOUT;
            if (parameters != null && parameters.Length > 0) new List<DbParameter>(parameters).ForEach(param => command.Parameters.Add(param));
            return command;
        }
        public override DbCommand CreateStoredProcedureCommand(string storedprocedure, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(storedprocedure)) return null;
            var command = new SQLiteCommand();
            command.CommandTimeout = COMMAND_TIMEOUT;
            command.CommandText = storedprocedure.Trim();
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null && parameters.Length > 0) new List<DbParameter>(parameters).ForEach(param => command.Parameters.Add(param));
            return command;
        }
        public override DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return command is SQLiteCommand ? new SQLiteDataAdapter(command as SQLiteCommand) : null;
        }

        public override long GenerateSequence(string sequence)
        {
            var sql = string.Empty;
            return this.Sequence(sql);
        }
    }
}
