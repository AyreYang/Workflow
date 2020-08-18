using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using DataBase.common.enums;
using DataBase.common.objects;
using DataBase.mssqlserver;
//using DataBase.postgresql;

namespace DataBase.common
{
    public abstract class TableEntityPage<T> : IDisposable where T : TableEntity, new()
    {
        private const int pagecount = 10;
        private const string SQL = "SELECT * FROM (SELECT {0} as PageNO, A.* FROM ({1}) A) B WHERE PageNO = {2}";

        protected DatabaseAccessor Accessor { get; private set; }
        public int PageCount { get; private set; }
        public Clause Clause { get; private set; }
        public Sort Sort { get; private set; }
        protected abstract string PageNOScript { get; }

        public TableEntityPage(Clause clause, Sort sort, int count, DatabaseAccessor accessor)
        {
            if (accessor == null) throw new Exception("DatabaseAccessor is null.");
            Accessor = accessor;
            PageCount = count > 0 ? count : pagecount;
            Clause = clause == null ? new Clause(string.Empty) : clause;
            Sort = sort == null ? new Sort() : sort;
        }

        public List<T> Retrieve(int pageno)
        {
            var list = new List<T>();
            if (pageno <= 0) return list;
            if (Sort.Count <= 0) return list;
            if (string.IsNullOrWhiteSpace(PageNOScript)) return list;
            string select = string.Empty;
            using (var entity = TableEntity.CreateEntity<T>())
            {
                select = (entity != null) ? entity.SQLTableSelect : null;
            }
            if (string.IsNullOrWhiteSpace(select)) return list;

            string where = null;
            List<DbParameter> parameters = null;
            Clause.Export(Accessor, out where, out parameters);
            if (!string.IsNullOrWhiteSpace(where)) select = string.Format("{0} WHERE {1}", select, where.ToString());

            var sql = string.Format(SQL, PageNOScript, select, pageno);

            string sort = null;
            Sort.Export(out sort);
            if (!string.IsNullOrWhiteSpace(sort)) sql = string.Format("{0} ORDER BY {1}", sql, sort);

            list = Accessor.RetrieveEntity<T>(Accessor.CreateCommand(sql, parameters), false);
            return list;
        }

        //private T CreateEntity()
        //{
        //    T entity = default(T);
        //    try
        //    {
        //        entity = typeof(T).Assembly.CreateInstance(typeof(T).ToString()) as T;
        //    }
        //    catch { }
        //    return entity;
        //}

        public void Dispose()
        {
            //Clauses.Clear();
            //Parameters.Clear();
            //Sorts.Clear();
        }
    }
}
