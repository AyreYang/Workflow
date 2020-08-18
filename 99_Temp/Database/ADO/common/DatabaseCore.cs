using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Database.ADO.interfaces;
using DataBase.common.interfaces;
using DataBase.common.objects;
using ServiceCore.Database.ADO.common;
using System.Data.SqlClient;
using System.Reflection;

namespace DataBase.common
{
    public abstract class DatabaseCore : IDatabase
    {
        private string ms_error = string.Empty;
        private Exception mex_error = null;
        private volatile object m_lock = new object();
        private List<DbCommand> mc_commands = new List<DbCommand>();

        protected IConnectionInfo mcnt_info = null;
        protected DbConnection msc_con = null;

        #region Abstract Methods
        public abstract string CreateParameterName(string name);
        public abstract DbParameter CreateParameter(DBColumn column);
        public abstract DbParameter CreateParameter(string name, object value);
        public abstract DbParameter CreateParameter(string name, SqlDbType type, int size, object value, ParameterDirection direction);
        public abstract DbCommand CreateCommand(string sql, params DbParameter[] parameters);
        public abstract DbCommand CreateCommand(string sql, List<DbParameter> parameters);

        public abstract DbCommand CreateStoredProcedureCommand(string storedprocedure, params DbParameter[] parameters);
        public abstract DbDataAdapter CreateDataAdapter(DbCommand command);
        public abstract bool TableExists(string as_table);
        public abstract long GenerateSequence(string sequence);

        public abstract DatabaseCore New();
        public abstract void Reset();
        //public abstract long ExecuteStoredProcedure(DbCommand command);
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
                    }else if (typeof(T).IsEnum)
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
        public string LastError
        {
            get { return ms_error; }
        }
        public Exception LastException
        {
            get { return mex_error; }
        }
        protected string RecordError(string as_method, Exception aex_error)
        {
            if (aex_error == null) return null;

            string ls_id = this.GetType().ToString();
            mex_error = aex_error;
            ms_error = string.Format("{0}:{1}:{2}", ls_id, as_method.Trim(), aex_error.Message);
            return ms_error;
        }
        protected void ClearError()
        {
            mex_error = null;
            ms_error = string.Empty;
        }
        protected long Sequence(string sql)
        {
            long def = -1;
            if (string.IsNullOrWhiteSpace(sql)) return def;
            return this.RetrieveValue<long>(CreateCommand(sql), def);
        }
        #endregion

        #region IDatabase Members
        public string ConnectString { get { return msc_con != null ? msc_con.ConnectionString : null; } }
        public void SetDBAccessor2(TableEntity entity)
        {
            if (entity != null) entity.SetDBAccessor(this);
        }
        public DataTable Retrieve(DbCommand command)
        {
            ClearError();

            if (command == null) return null;

            DataTable ldt_result = null;
            string ls_query = string.Empty;
            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();

                using (command)
                {
                    ls_query = command.CommandText;
                    if (string.IsNullOrEmpty(ls_query)) return null;
                    ls_query = ls_query.Trim();
                    if (command.CommandType == CommandType.Text && !ls_query.ToUpper().StartsWith("SELECT")) return null;

                    command.Connection = msc_con;
                    using (DbDataReader lsdr_reader = command.ExecuteReader())
                    {
                        ldt_result = new DataTable();
                        ldt_result.Load(lsdr_reader);
                    }
                }
                return ldt_result;
            }
            catch (System.Exception lex_err)
            {
                RecordError("Retrieve", lex_err);
                return null;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public DataTable Retrieve(string sql, params DbParameter[] parameters)
        {
            var command = CreateCommand(sql, parameters);
            return Retrieve(command);
        }
        public void RetrieveReader(DbCommand command, Action<DbDataReader> action)
        {
            ClearError();

            if (command == null || action == null) return;

            string ls_query = string.Empty;
            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();

                using (command)
                {
                    command.Connection = msc_con;
                    using (DbDataReader lsdr_reader = command.ExecuteReader())
                    {
                        action(lsdr_reader);
                    }
                }
            }
            catch (System.Exception lex_err)
            {
                RecordError("Retrieve", lex_err);
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
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
        public long ImportTable<T1, T2>(IDatabase from, bool truncate)
            where T1:TableEntity, new()
            where T2:TableEntity, new()
        {
            ClearError();

            if (from == null) return -1;

            var table1 = new T1();
            var table2 = new T2();

            long ll_rows = 0;
            try
            {
                if (!from.TableExists(table1.TableName)) return -2;
                if (!this.TableExists(table2.TableName))
                {
                    //table2.SetDBAccessor(this.New());
                    table2.SetDBAccessor(this);
                    if (!table2.CreateTable()) return -3;
                }

                //if (!msc_con.State.Equals(ConnectionState.Closed)) this.Reset();
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                from.RetrieveReader(from.CreateCommand(table1.SQLTableSelect), (reader) =>
                {
                    var t1 = reader.GetSchemaTable();
                    var table = new DataTable();
                    for (int f = 0; f < reader.FieldCount; f++)
                    {
                        table.Columns.Add(reader.GetName(f), reader.GetFieldType(f));
                    }
                    using (DbTransaction lst_trans = msc_con.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            if (truncate)
                            {
                                using (var command = this.CreateCommand(table2.SQLTableTruncate))
                                {
                                    command.Connection = msc_con;
                                    command.Transaction = lst_trans;
                                    var v = command.ExecuteNonQuery();
                                }
                            }
                            while (reader.Read())
                            {
                                var values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                var row = table.LoadDataRow(values, true);
                                var entity1 = new T1();
                                entity1.SetEntity(row);
                                var entity2 = new T2();
                                entity2.SetEntity(entity1);

                                using (var command = entity2.GetInsertCommand(this))
                                {
                                    command.Connection = msc_con;
                                    command.Transaction = lst_trans;
                                    if (command.ExecuteNonQuery() > 0) ll_rows++;
                                }

                                table.Clear();
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
                });
                
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
        public long ImportTable(TableSchama source, TableSchama target, bool truncate)
        {
            ClearError();

            if (source == null) throw new ArgumentNullException("parameter(source) is null!", "source");
            if (target == null) throw new ArgumentNullException("parameter(target) is null!", "target");
            if (source.TYPE == enums.TableSchamaType.TABLE && !source.Exists) throw new ArgumentException(string.Format("parameter(source:{0}) is not a existed table!", source.TableName));
            if (!source.Ready) throw new ArgumentException("parameter(source) is not a ready schama!");
            if (target.TYPE != enums.TableSchamaType.TABLE) throw new ArgumentException("parameter(target) is not a table type schama!");

            long ll_rows = 0;
            try
            {
                if (!target.Exists && !target.CreateTableBySchama(source)) return -1;

                source.RetrieveReader(reader =>
                {
                    if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                    using (DbTransaction lst_trans = msc_con.BeginTransaction(IsolationLevel.ReadCommitted))
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
                            while (reader.Read())
                            {
                                var values = new object[reader.FieldCount];
                                reader.GetValues(values);
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
                });
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

        public long ImportTable(DbDataReader source, string table, bool truncate)
        {
            ClearError();

            if (source == null) throw new ArgumentNullException("parameter(source) is null!", "source");
            if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException("parameter(table) is null!", "table");

            long ll_rows = 0;
            try
            {
                if (!this.TableExists(table)) return -1;

                if (truncate) ExecuteSQLCommand(CreateCommand(string.Format("TRUNCATE TABLE {0}", table.Trim())));

                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (SqlBulkCopy blkCpyBackup = new SqlBulkCopy(this.ConnectString, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls))
                {
                    blkCpyBackup.DestinationTableName = table.Trim();
                    blkCpyBackup.BulkCopyTimeout = 600;
                    blkCpyBackup.BatchSize = 5000;
                    blkCpyBackup.WriteToServer(source);

                    var info = typeof(SqlBulkCopy).GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                    ll_rows = (int)info.GetValue(blkCpyBackup);
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

        public long ImportTable(DbDataReader source, TableSchama target, bool truncate)
        {
            ClearError();

            if (source == null) throw new ArgumentNullException("parameter(source) is null!", "source");
            if (target == null) throw new ArgumentNullException("parameter(target) is null!", "target");
            if (target.TYPE != enums.TableSchamaType.TABLE) throw new ArgumentException("parameter(target) is not a TableSchama!");

            long ll_rows = 0;
            try
            {
                if (!target.Exists) return -1;

                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (DbTransaction lst_trans = msc_con.BeginTransaction(IsolationLevel.ReadCommitted))
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
        public List<T> RetrieveEntity<T>(DbCommand command) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(command, true);
        }
        public List<T> RetrieveEntity<T>() where T : TableEntity, new()
        {
            return RetrieveEntity<T>(null, null);
        }
        public List<T> RetrieveEntity<T>(Clause clause) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(clause, null);
        }
        public List<T> RetrieveEntity<T>(Sort sort) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(null, sort);
        }
        public List<T> RetrieveEntity<T>(Clause clause, Sort sort) where T : TableEntity, new()
        {
            var sql = new StringBuilder();
            var entity = new T();
            sql.AppendLine(entity.SQLTableSelect);

            List<DbParameter> parameters = null;
            string txtClause = null;
            if (clause != null) clause.Export(this, out txtClause, out parameters);
            if (!string.IsNullOrWhiteSpace(txtClause)) sql.AppendLine(" WHERE " + txtClause);

            string txtSort = null;
            if (sort != null) sort.Export(out txtSort);
            if (!string.IsNullOrWhiteSpace(txtSort)) sql.AppendLine(" ORDER BY " + txtSort);

            var command = CreateCommand(sql.ToString(), parameters);
            return RetrieveEntity<T>(command, false);
        }
        public T RetrieveValue<T>(DbCommand command, T def = default(T))
            //where T : struct
        {
            ClearError();

            if (command == null) return def;

            try
            {
                var value = def;
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (command)
                {
                    var ls_query = command.CommandText;
                    if (string.IsNullOrEmpty(ls_query)) return def;
                    ls_query = ls_query.Trim();
                    if (!ls_query.ToUpper().StartsWith("SELECT")) return def;

                    command.Connection = msc_con;
                    value = Convert2<T>(command.ExecuteScalar());
                }
                return value;
            }
            catch (Exception lex_err)
            {
                RecordError("RetrieveValue", lex_err);
                return def;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public long CountInTable(string table, Clause clause = null)
        {
            ClearError();
            if (string.IsNullOrWhiteSpace(table)) return -1;
            var sql = new StringBuilder();
            sql.AppendLine(string.Format("SELECT COUNT(1) FROM {0}", table.Trim().ToUpper()));

            List<DbParameter> parameters = null;
            string txtClause = null;
            if (clause != null) clause.Export(this, out txtClause, out parameters);
            if (!string.IsNullOrWhiteSpace(txtClause)) sql.AppendLine(" WHERE " + txtClause);

            return RetrieveValue<long>(CreateCommand(sql.ToString(), parameters));
        }



        //public long SaveTableEntity(TableEntity entity)
        //{
        //    ClearError();

        //    if (entity == null) return 0;
        //    long ll_ret = 0;

        //    try
        //    {
        //        if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
        //        using (DbTransaction lst_trans = msc_con.BeginTransaction())
        //        {
        //            try
        //            {
        //                // Save Self
        //                List<DbCommand> commands = null;
        //                switch (entity.EntityStatus)
        //                {
        //                    case EntityState.ASSIGNED:
        //                        commands = entity.GetInsertCommands(this);
        //                        break;
        //                    case EntityState.CHANGED:
        //                        commands = entity.GetUpdateCommands(this);
        //                        break;
        //                    default:
        //                        commands = null;
        //                        break;
        //                }

        //                if (commands != null)
        //                {
        //                    foreach (DbCommand command in commands)
        //                    {
        //                        if (command == null) throw new System.Exception("Command object is invalidate.");
        //                        using (command)
        //                        {
        //                            command.Connection = msc_con;
        //                            command.Transaction = lst_trans;
        //                            var v = command.ExecuteNonQuery();
        //                            if (v > 0) ll_ret++;
        //                        }
        //                    }
        //                }


        //                // Save Foreigns
        //                commands = new List<DbCommand>();
        //                if (entity.EntityStatus > EntityState.RAW)
        //                {
        //                    // Save Foreigns
        //                    (new List<DBForeign>(entity.Foreigns.Values)).ForEach(foreign =>
        //                    {
        //                        commands.AddRange(foreign.GetSaveCommands(this));
        //                    });
        //                }
        //                if (commands.Count > 0)
        //                {
        //                    foreach (DbCommand command in commands)
        //                    {
        //                        if (command == null) throw new System.Exception("Command object is invalidate.");
        //                        using (command)
        //                        {
        //                            command.Connection = msc_con;
        //                            command.Transaction = lst_trans;
        //                            var v = command.ExecuteNonQuery();
        //                            if (v > 0) ll_ret++;
        //                        }
        //                    }
        //                }

        //                lst_trans.Commit();
        //            }
        //            catch (Exception err)
        //            {
        //                WriteError("ExecuteSQLCommand", err.Message);

        //                if (lst_trans != null)
        //                {
        //                    lst_trans.Rollback();
        //                }
        //                return -1;
        //            }
        //        }

        //        return ll_ret;

        //    }
        //    catch (System.Exception lecp_err)
        //    {
        //        WriteError("ExecuteSQLCommand", lecp_err.Message);
        //        return -1;
        //    }
        //    finally
        //    {
        //        if (msc_con != null) msc_con.Close();
        //    }
        //}
        public long ExecuteSQLCommand(DbCommand command)
        {
            ClearError();

            if (command == null) return 0;
            long ll_ret = 0;
            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();

                using (DbTransaction lst_trans = msc_con.BeginTransaction())
                {
                    try
                    {
                        using (command)
                        {
                            command.Connection = msc_con;
                            command.Transaction = lst_trans;
                            ll_ret = command.ExecuteNonQuery();
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        RecordError("ExecuteSQLCommand", err);
                        if (lst_trans != null)
                        {
                            lst_trans.Rollback();
                        }
                        return -1;
                    }
                }

                return ll_ret <= 0 ? 0 : ll_ret;

            }
            catch (System.Exception lex_err)
            {
                RecordError("ExecuteSQLCommand", lex_err);
                return -1;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public long ExecuteSQLCommand(List<DbCommand> commands)
        {
            ClearError();

            if (commands == null || commands.Count == 0) return 0;
            long ll_ret = 0;

            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (DbTransaction lst_trans = msc_con.BeginTransaction())
                {
                    try
                    {
                        foreach (DbCommand command in commands)
                        {
                            if (command == null) throw new System.Exception("Command object is invalidate.");
                            using (command)
                            {
                                command.Connection = msc_con;
                                command.Transaction = lst_trans;
                                var v = command.ExecuteNonQuery();
                                if (v > 0) ll_ret++;
                            }
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        RecordError("ExecuteSQLCommand", err);

                        if (lst_trans != null)
                        {
                            lst_trans.Rollback();
                        }
                        return -1;
                    }
                }

                return ll_ret;

            }
            catch (System.Exception lecp_err)
            {
                RecordError("ExecuteSQLCommand", lecp_err);
                return -1;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public long ExecuteSQLCommand(List<IADbCommand> commands)
        {
            ClearError();

            if (commands == null || commands.Count == 0) return 0;
            long ll_ret = 0;

            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();
                using (DbTransaction lst_trans = msc_con.BeginTransaction())
                {
                    try
                    {
                        foreach (IADbCommand command in commands)
                        {
                            if (command == null || command.Command == null) throw new System.Exception("Command object is invalidate.");
                            var v = command.Execute(msc_con, lst_trans, commands.Select(c => c.Command).ToList());
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        RecordError("ExecuteSQLCommand", err);

                        if (lst_trans != null)
                        {
                            lst_trans.Rollback();
                        }
                        return -1;
                    }
                }

                return ll_ret;

            }
            catch (System.Exception lecp_err)
            {
                RecordError("ExecuteSQLCommand", lecp_err);
                return -1;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }

        public long ExecuteStoredProcedure(string storedprocedure, params DbParameter[] parameters)
        {
            ClearError();

            long ll_ret = 0;
            string ls_method = "ExecuteStoredProcedure";
            var command = CreateStoredProcedureCommand(storedprocedure, parameters);
            if (command == null || command.CommandType != CommandType.StoredProcedure) return ll_ret;

            try
            {
                if (!msc_con.State.Equals(ConnectionState.Open)) msc_con.Open();

                using (DbTransaction lst_trans = msc_con.BeginTransaction())
                {
                    try
                    {
                        using (command)
                        {
                            command.Connection = msc_con;
                            command.Transaction = lst_trans;
                            ll_ret = command.ExecuteNonQuery();
                        }

                        lst_trans.Commit();
                    }
                    catch (Exception err)
                    {
                        RecordError(ls_method, err);
                        if (lst_trans != null)
                        {
                            lst_trans.Rollback();
                        }
                        return -1;
                    }
                }

                return ll_ret <= 0 ? 0 : ll_ret;

            }
            catch (System.Exception lex_err)
            {
                RecordError(ls_method, lex_err);
                return -1;
            }
            finally
            {
                if (msc_con != null) msc_con.Close();
            }
        }
        public List<T> ExecuteStoredProcedure<T>(string storedprocedure, params DbParameter[] parameters) where T : TableEntity, new()
        {
            return RetrieveEntity<T>(CreateStoredProcedureCommand(storedprocedure, parameters));
        }


        public void InsertEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.INTERNAL_GetInsertCommands(this));
        }
        public void UpdateEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.INTERNAL_GetUpdateCommands(this));
        }
        public void SaveEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.INTERNAL_GetSaveCommands(this));
        }
        public void DeleteEntity(params TableEntity[] list)
        {
            if (list == null || list.Length <= 0) return;
            foreach (TableEntity ent in list) if (ent != null) AddCommands(ent.INTERNAL_GetDeleteCommands(this));
        }
        public long Commit()
        {
            long result = 0;
            lock (m_lock)
            {
                result = ExecuteSQLCommand(mc_commands);
                mc_commands.Clear();
            }
            return result;
        }
        #endregion

        #region Private Methods
        private void AddCommands(List<DbCommand> commands)
        {
            if(commands == null || commands.Count <= 0)return;
            lock (m_lock)
            {
                mc_commands.AddRange(commands);
            }
        }
        internal List<T> RetrieveEntity<T>(DbCommand command, bool needfresh) where T : TableEntity, new()
        {
            var result = new List<T>();
            var data = Retrieve(command);
            if (data != null && data.Rows.Count > 0)
            {
                var ticks = DateTime.Now.Ticks;
                foreach (DataRow row in data.Rows)
                {
                    var ent = new T();
                    ent.SetDBAccessor(this);
                    if (needfresh)
                    {
                        ent.SetEntity(row);
                        ent.Fresh();
                    }
                    else
                    {
                        ent.SetEntity(row, ticks);
                    }
                    result.Add(ent);
                }
            }
            return result;
        }
        #endregion

        public void Dispose()
        {
            ClearError();
            try
            {
                if (msc_con != null)
                {
                    if (!msc_con.State.Equals(ConnectionState.Closed)) msc_con.Close();
                    msc_con.Dispose();
                }
            }
            catch (System.Exception lept_err)
            {
                RecordError("Dispose", lept_err);
            }
            finally
            {
                msc_con = null;
            }
        }

        ~DatabaseCore()
        {
            mcnt_info = null;
            Dispose();
        }
        
    }
}
