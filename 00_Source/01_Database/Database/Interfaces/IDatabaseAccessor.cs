using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Database.Interfaces
{
    public interface IDatabaseAccessor
    {
        string ConnectString { get; }
        DataTable Retrieve(string sql, params DbParameter[] parameters);
        DataTable Retrieve(DbCommand command);

        DataSet RetrieveDataSet(DbCommand command);

        //void RetrieveReader(DbCommand command, Action<DataTable, int, bool> action, int cache);
        //void RetrieveReader(DbCommand command, Action<DbDataReader> action);

        //DbType Convert2DbType(string type);

        //long ImportTable(DbDataReader source, string table, bool truncate);

        //List<T> Retrieve<T>(DbCommand command, bool ignoreCase = true) where T : new();
        //T RetrieveValue<T>(DbCommand command, T def = default(T));
        //where T : struct;
        long ExecuteSQLCommand(params DbCommand[] commands);
        long ExecuteStoredProcedure(string storedprocedure, params DbParameter[] parameters);

        long GenerateSequence(string sequence);

        string CreateParameterName(string name);
        DbParameter CreateParameter(string name, object value);
        DbParameter CreateParameter(string name, DbType type, int size, object value, ParameterDirection direction);
        DbCommand CreateCommand(string sql, params DbParameter[] parameters);
        DbCommand CreateStoredProcedureCommand(string storedprocedure, params DbParameter[] parameters);
        DbDataAdapter CreateDataAdapter(DbCommand command);
        //bool TableExists(string as_table);

    }
}
