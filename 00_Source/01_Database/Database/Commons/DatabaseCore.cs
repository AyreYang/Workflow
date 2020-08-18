using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Database.Commons
{
    public abstract class DatabaseCore : IDatabaseAccessor
    {
        //private volatile object m_lock = new object();
        //private List<DbCommand> mc_commands = new List<DbCommand>();

        protected IConnectionInfo mcnt_info = null;


        #region Abstract Methods
        public abstract string CreateParameterName(string name);
        public abstract DbParameter CreateParameter(string name, object value);
        public abstract DbParameter CreateParameter(string name, DbType type, int size, object value, ParameterDirection direction);
        public abstract DbCommand CreateCommand(string sql, params DbParameter[] parameters);

        public abstract DbCommand CreateStoredProcedureCommand(string storedprocedure, params DbParameter[] parameters);
        public abstract DbDataAdapter CreateDataAdapter(DbCommand command);
        //public abstract bool TableExists(string as_table);
        public abstract long GenerateSequence(string sequence);

        //public abstract DatabaseCore New();
        //public abstract void Reset();

        protected abstract DbConnection CreateConnection(string cnnstr);
        //public abstract IAccessor Clone(params KeyValuePair<string, object>[] args);
        #endregion

        #region Static Methods
        public static T Convert2<T>(object val, T def = default(T))
        {
            var result = false;
            return Convert2<T>(val, out result, def);
        }
        public static T Convert2<T>(object val, out bool result, T def = default(T))
        {
            result = false;
            if (val == null)
            {
                result = typeof(T).Name.ToUpper().StartsWith("NULLABLE") || typeof(T).IsClass;
                return result ? default(T) : def;
            }
            else
            {
                object value = null;
                try
                {
                    if (val.GetType() == typeof(T))
                    {
                        value = val;
                    }
                    else if (typeof(T).IsEnum)
                    {
                        value = Convert.ToInt32(val);
                        value = (result = (Enum.IsDefined(typeof(T), value))) ? (T)value : default(T);
                    }
                    else
                    {
                        switch (typeof(T).ToString())
                        {
                            case "System.Boolean":
                                value = Convert.ToBoolean(val);
                                break;
                            case "System.Char":
                                value = Convert.ToChar(val);
                                break;
                            case "System.DateTime":
                                value = Convert.ToDateTime(val);
                                break;
                            case "System.Int16":
                                value = Convert.ToInt16(val);
                                break;
                            case "System.Int32":
                                value = Convert.ToInt32(val);
                                break;
                            case "System.Int64":
                                value = Convert.ToInt64(val);
                                break;
                            case "System.Decimal":
                                value = Convert.ToDecimal(val);
                                break;
                            case "System.String":
                                value = Convert.ToString(val);
                                break;
                            case "System.Object":
                                value = val;
                                break;
                            default:
                                value = (T)val;
                                break;
                        }
                    }
                }
                catch { }

                result = (value != null);
                return result ? (T)value : def;
            }

        }
        public static T GetValueFormDataRow<T>(DataRow row, string column, T def = default(T))
        {
            if (row == null || string.IsNullOrWhiteSpace(column)) return def;
            return Convert2<T>(row[column.Trim()], def);
        }
        #endregion

        #region Member Methods

        protected long Sequence(string sql)
        {
            long def = -1;
            if (string.IsNullOrWhiteSpace(sql)) return def;
            return this.RetrieveValue<long>(CreateCommand(sql), def);
        }
        #endregion

        #region IDatabase Members
        public string ConnectString { get { return mcnt_info != null ? mcnt_info.DBConString : null; } }
        public DataTable Retrieve(DbCommand command)
        {
            if (command == null) return null;

            DataTable ldt_result = null;
            string ls_query = string.Empty;

            using (var conn = CreateConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();

                using (command)
                {
                    ls_query = command.CommandText;
                    if (string.IsNullOrWhiteSpace(ls_query)) return null;
                    ls_query = ls_query.Trim();
                    if (command.CommandType == CommandType.Text && !ls_query.ToUpper().StartsWith("SELECT")) return null;

                    command.Connection = conn;
                    using (DbDataReader lsdr_reader = command.ExecuteReader())
                    {
                        ldt_result = new DataTable();
                        ldt_result.Load(lsdr_reader);
                    }
                }
            }

            return ldt_result;
        }
        public DataTable Retrieve(string sql, params DbParameter[] parameters)
        {
            return Retrieve(CreateCommand(sql, parameters));
        }
        public DataSet RetrieveDataSet(DbCommand command)
        {
            if (command == null) return null;

            DataSet lds_result = null;

            using (var conn = CreateConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();
                using (command)
                {
                    command.Connection = conn;
                    using (var adapter = CreateDataAdapter(command))
                    {
                        lds_result = new System.Data.DataSet();
                        adapter.Fill(lds_result);
                    }
                }
            }

            return lds_result;
        }
        public void RetrieveReader(DbCommand command, Action<DbDataReader> action)
        {
            if (command == null || action == null) return;

            using (var conn = CreateConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();

                using (command)
                {
                    command.Connection = conn;
                    using (DbDataReader lsdr_reader = command.ExecuteReader())
                    {
                        action(lsdr_reader);
                    }
                }
            }
        }
        public void RetrieveReader(DbCommand command, Action<DataTable, int, bool> action, int cache)
        {
            RetrieveReader(command, (reader) =>
            {
                //DataTable table = new DataTable();
                if (cache <= 0)
                {
                    using (DataTable table = new DataTable())
                    {
                        table.Load(reader);
                        action(table, 1, true);
                    }
                }
                else
                {
                    DataTable table = null;
                    bool flag = false;
                    int batch = 0;
                    int fcount = reader.FieldCount;
                    do
                    {
                        object[] values = null;
                        if (flag = reader.Read())
                        {
                            values = new object[fcount];
                            reader.GetValues(values);

                            if (table == null)
                            {
                                table = new DataTable();
                                for (int f = 0; f < fcount; f++)
                                {
                                    table.Columns.Add(reader.GetName(f), reader.GetFieldType(f));
                                }
                            }
                            table.LoadDataRow(values, true);
                        }

                        if (!flag || table.Rows.Count >= cache)
                        {
                            using (table)
                            {
                                action(table, ++batch, !reader.HasRows);
                                table.Rows.Clear();
                            }
                            table = null;
                            GC.Collect();
                        }

                    } while (flag);
                }
            });

        }
        /*
        public long ImportTable(DbDataReader source, string table, bool truncate)
        {
            if (source == null) throw new ArgumentNullException("parameter(source) is null!", "source");
            if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException("parameter(table) is null!", "table");

            if (!this.TableExists(table)) return -1;

            long ll_rows = 0;
            using (var conn = new SqlConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (truncate)
                        {
                            using (var command = CreateCommand(string.Format("TRUNCATE TABLE {0}", table.Trim())))
                            {
                                command.Connection = conn;
                                command.Transaction = trans;
                                command.ExecuteNonQuery();
                            }
                        }

                        using (SqlBulkCopy blkCpyBackup = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls, trans))
                        {
                            blkCpyBackup.DestinationTableName = table.Trim();
                            blkCpyBackup.BulkCopyTimeout = 600;
                            blkCpyBackup.BatchSize = 5000;
                            blkCpyBackup.WriteToServer(source);

                            var info = typeof(SqlBulkCopy).GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                            ll_rows = (int)info.GetValue(blkCpyBackup);
                        }


                        trans.Commit();
                    }
                    catch (Exception err)
                    {
                        if (trans != null) trans.Rollback();
                        throw err;
                    }
                }
            }

            return ll_rows;
        }*/

        /*
        public long ImportTable(DbDataReader source, TableSchama target, bool truncate)
        {
            if (source == null) throw new ArgumentNullException("parameter(source) is null!", "source");
            if (target == null) throw new ArgumentNullException("parameter(target) is null!", "target");
            if (target.TYPE != enums.TableSchamaType.TABLE) throw new ArgumentException("parameter(target) is not a TableSchama!");

            long ll_rows = 0;
            try
            {
                if (!target.Exists) return -1;

                using (var conn = CreateConnection(ConnectString))
                {
                    if (!conn.State.Equals(ConnectionState.Open)) conn.Open();
                    using (DbTransaction lst_trans = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            if (truncate)
                            {
                                using (var command = target.GetTruncateCommand())
                                {
                                    command.Connection = msc_con;
                                    command.Transaction = lst_trans;
                                    var v = command.ExecuteNonQuery();
                                }
                            }
                            while (source.Read())
                            {
                                var values = new object[source.FieldCount];
                                source.GetValues(values);
                                using (var command = target.GetInsertCommand(values))
                                {
                                    command.Connection = msc_con;
                                    command.Transaction = lst_trans;
                                    if (command.ExecuteNonQuery() > 0) ll_rows++;
                                }
                            }

                            lst_trans.Commit();
                        }
                        catch (Exception err)
                        {
                            if (lst_trans != null)
                            {
                                lst_trans.Rollback();
                            }
                            throw err;
                        }
                    }
                }
                
            }
            catch (System.Exception lex_err)
            {
                RecordError("ImportTable", lex_err);
                return -4;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
            return ll_rows;
        }
        */
        public List<T> Retrieve<T>(DbCommand command, bool ignoreCase = true) where T : new()
        {
            List<T> result = new List<T>();
            DataTable data = null;
            if (command != null && (data = Retrieve(command)) != null && data.Rows.Count > 0)
            {
                var dict = new Dictionary<string, System.Reflection.PropertyInfo>();
                var properties = (typeof(T)).GetProperties();
                foreach (DataColumn col in data.Columns)
                {
                    var name = ignoreCase ? col.ColumnName.Trim().ToUpper() : col.ColumnName;
                    var info = ignoreCase ? properties.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals(name)) : properties.FirstOrDefault(p => p.Name.Equals(name));
                    if (info != null) dict.Add(col.ColumnName, info);
                }
                if (dict.Count > 0)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        var ent = new T();
                        dict.Keys.ToList().ForEach(key =>
                        {
                            dict[key].SetValue(ent, row[key], null);
                        });
                        result.Add(ent);
                    }
                }
            }
            return result;
        }
        public T RetrieveValue<T>(DbCommand command, T def = default(T))
        {
            if (command == null) return def;

            var value = def;

            using (var conn = CreateConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();

                using (command)
                {
                    var ls_query = command.CommandText;
                    if (string.IsNullOrEmpty(ls_query)) return def;
                    ls_query = ls_query.Trim();
                    if (!ls_query.ToUpper().StartsWith("SELECT")) return def;

                    command.Connection = conn;
                    value = Convert2<T>(command.ExecuteScalar());
                }
            }

            return value;
        }
        //public long CountInTable(string table, Clause clause = null)
        //{
        //    if (string.IsNullOrWhiteSpace(table)) return -1;
        //    var sql = new StringBuilder();
        //    sql.AppendLine(string.Format("SELECT COUNT(1) FROM {0}", table.Trim().ToUpper()));

        //    List<DbParameter> parameters = null;
        //    string txtClause = null;
        //    if (clause != null) clause.Export(this, out txtClause, out parameters);
        //    if (!string.IsNullOrWhiteSpace(txtClause)) sql.AppendLine(" WHERE " + txtClause);

        //    return RetrieveValue<long>(CreateCommand(sql.ToString(), parameters.ToArray()));
        //}
        public DbType Convert2DbType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException("type");

            switch (type.Trim().ToLower())
            {
                case "short":
                case "int16":
                    return DbType.Int16;
                case "int":
                case "int32":
                    return DbType.Int32;
                case "long":
                case "int64":
                    return DbType.Int64;
                case "astr":
                case "astring":
                    return DbType.AnsiString;
                case "str":
                case "string":
                    return DbType.String;
                case "decimal":
                    return DbType.Decimal;
                case "guid":
                case "ui":
                case "uniq":
                case "unique":
                case "uniqid":
                case "uniqueidentifier":
                    return DbType.Guid;
                case "date":
                    return DbType.Date;
                case "datetime":
                    return DbType.DateTime;
                case "bool":
                    return DbType.Boolean;
                case "byte":
                case "char":
                    return DbType.Byte;
            }

            throw new ArgumentException(string.Format("Unsupported type({0})!", type.Trim()), "type");
        }

        public long ExecuteSQLCommand(params DbCommand[] commands)
        {
            if (commands == null || commands.Length == 0) return 0;
            long ll_ret = 0;
            using (var conn = CreateConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();
                using (DbTransaction lst_trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (DbCommand command in commands)
                        {
                            if (command == null) throw new ArgumentException("Contains null command object in the commands list!", "commands");
                            using (command)
                            {
                                command.Connection = conn;
                                command.Transaction = lst_trans;
                                var v = command.ExecuteNonQuery();
                                if (v > 0) ll_ret++;
                            }
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        if (lst_trans != null) lst_trans.Rollback();
                        throw err;
                    }
                }
            }

            return ll_ret;
        }

        public long ExecuteStoredProcedure(string storedprocedure, params DbParameter[] parameters)
        {
            long ll_ret = 0;
            var command = CreateStoredProcedureCommand(storedprocedure, parameters);
            if (command == null || command.CommandType != CommandType.StoredProcedure) return ll_ret;
            using (var conn = CreateConnection(ConnectString))
            {
                if (!conn.State.Equals(ConnectionState.Open)) conn.Open();
                using (DbTransaction lst_trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (command)
                        {
                            command.Connection = conn;
                            command.Transaction = lst_trans;
                            ll_ret = command.ExecuteNonQuery();
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        if (lst_trans != null) lst_trans.Rollback();
                        throw err;
                    }
                }
            }
            return ll_ret <= 0 ? 0 : ll_ret;
        }

        #endregion

    }
}
