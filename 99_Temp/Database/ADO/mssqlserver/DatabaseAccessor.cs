using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using DataBase.common;
using DataBase.common.messages;
using DataBase.common.objects;

namespace DataBase.mssqlserver
{
    sealed public class DatabaseAccessor : DatabaseCore
    {
        private const string PREFIX_PARM = "@";
        public const int COMMAND_TIMEOUT = 600;

        public DatabaseAccessor(string host, string database, string user, string password)
        {
            mcnt_info = new ConnectionInfo(host, database, user, password);
            msc_con = new SqlConnection(mcnt_info.DBConString);
        }

        public override bool TableExists(string as_table)
        {
            if (string.IsNullOrWhiteSpace(as_table)) return false;
            var count = CountInTable("SysObjects", new Clause("type = 'U' AND name = {name}").AddParam("name", as_table.Trim()));
            return count > 0;
        }

        public override string CreateParameterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return PREFIX_PARM + name.Trim().ToUpper();
        }

        public override DbParameter CreateParameter(DBColumn column)
        {
            if (column == null) return null;
            return CreateParameter(column.ID, column.Value);
        }
        public override DbParameter CreateParameter(string name, object value)
        {
            var pname = CreateParameterName(name);
            if (string.IsNullOrWhiteSpace(pname)) return null;
            return new SqlParameter(pname, value == null ? DBNull.Value : value);
        }
        public override DbParameter CreateParameter(string name, SqlDbType type, int size, object value, ParameterDirection direction)
        {
            var pname = CreateParameterName(name);
            if (string.IsNullOrWhiteSpace(pname)) return null;
            DbParameter parameter = new SqlParameter(pname, type, size);
            parameter.Value = value == null ? DBNull.Value : value;
            parameter.Direction = direction;
            return parameter;
        }
        public override DbCommand CreateCommand(string sql, List<DbParameter> parameters)
        {
            return (parameters != null && parameters.Count > 0) ? CreateCommand(sql, parameters.ToArray()) : CreateCommand(sql, new DbParameter[0]);
        }
        public override DbCommand CreateCommand(string sql, params DbParameter[] parameters)
        {
            var command = new SqlCommand(sql);
            command.CommandTimeout = COMMAND_TIMEOUT;
            if (parameters != null && parameters.Length > 0) new List<DbParameter>(parameters).ForEach(param => command.Parameters.Add(param));
            return command;
        }
        public override DbCommand CreateStoredProcedureCommand(string storedprocedure, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(storedprocedure)) return null;
            var command = new SqlCommand();
            command.CommandTimeout = COMMAND_TIMEOUT;
            command.CommandText = storedprocedure.Trim();
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null && parameters.Length > 0) new List<DbParameter>(parameters).ForEach(param => command.Parameters.Add(param));
            return command;
        }
        public override DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return new SqlDataAdapter(command as SqlCommand);
        }

        public override long GenerateSequence(string sequence)
        {
            var sql = string.Empty;
            return this.Sequence(sql);
        }



        public override DatabaseCore New()
        {
            var cnt_info = mcnt_info as ConnectionInfo;
            return cnt_info != null ? new DatabaseAccessor(cnt_info.Host, cnt_info.Database, cnt_info.User, cnt_info.Password) : null;
        }

        public override void Reset()
        {
            msc_con = new SqlConnection(mcnt_info.DBConString);
        }
    }
}
