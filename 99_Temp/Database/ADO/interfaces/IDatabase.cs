using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Database.ADO.interfaces;
using DataBase.common.objects;
using ServiceCore.Database.ADO.common;

namespace DataBase.common.interfaces
{
    public interface IDatabase : IDisposable
    {
        string ConnectString { get; }
        string LastError { get; }
        Exception LastException { get; }
        DataTable Retrieve(string sql, params DbParameter[] parameters);
        DataTable Retrieve(DbCommand command);
        void RetrieveReader(DbCommand command, Action<DataTable, int, bool> action, int cache);
        void RetrieveReader(DbCommand command, Action<DbDataReader> action);

        long ImportTable<T1, T2>(IDatabase from, bool truncate)
            where T1 : TableEntity, new()
            where T2 : TableEntity, new();
        long ImportTable(TableSchama source, TableSchama target, bool truncate);
        long ImportTable(DbDataReader source, string table, bool truncate);

        List<T> Retrieve<T>(DbCommand command, bool ignoreCase = true) where T : new();
        List<T> RetrieveEntity<T>(Clause clause, Sort sort) where T : TableEntity, new();
        List<T> RetrieveEntity<T>(DbCommand command) where T : TableEntity, new();
        T RetrieveValue<T>(DbCommand command, T def = default(T));
            //where T : struct;
        long ExecuteSQLCommand(DbCommand command);
        long ExecuteSQLCommand(List<DbCommand> commands);
        long ExecuteSQLCommand(List<IADbCommand> commands);
        long ExecuteStoredProcedure(string storedprocedure, params DbParameter[] parameters);
        List<T> ExecuteStoredProcedure<T>(string storedprocedure, params DbParameter[] parameters) where T : TableEntity, new();

        long GenerateSequence(string sequence);

        void SetDBAccessor2(TableEntity entity);
        void InsertEntity(params TableEntity[] list);
        void UpdateEntity(params TableEntity[] list);
        void SaveEntity(params TableEntity[] list);
        void DeleteEntity(params TableEntity[] list);
        long Commit();

        string CreateParameterName(string name);
        DbParameter CreateParameter(DBColumn column);
        DbParameter CreateParameter(string name, object value);
        DbParameter CreateParameter(string name, SqlDbType type, int size, object value, ParameterDirection direction);
        DbCommand CreateCommand(string sql, params DbParameter[] parameters);
        DbCommand CreateCommand(string sql, List<DbParameter> parameters);
        DbCommand CreateStoredProcedureCommand(string storedprocedure, params DbParameter[] parameters);
        DbDataAdapter CreateDataAdapter(DbCommand command);
        bool TableExists(string as_table);

    }
}
