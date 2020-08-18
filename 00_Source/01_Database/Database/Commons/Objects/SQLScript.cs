using Database.Commons.Objects.Enums;
using Database.Commons.Objects.SQLItems;
using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects
{
    public class SQLScript : SQLContainer
    {
        private Type[] TYPE_SELECT = new Type[] { typeof(SelectItem), typeof(FromItem), typeof(JoinItem), typeof(WhereItem) };
        private Type[] TYPE_INSERT = new Type[] { typeof(InsertItem) };
        private Type[] TYPE_UPDATE = new Type[] { typeof(UpdateItem), typeof(WhereItem) };
        private Type[] TYPE_DELETE = new Type[] { typeof(DeleteItem), typeof(WhereItem) };
        public string ID { get; private set; }
        
        private IList<SQLItem> _list { get; set; }

        public SQLTYPE TYPE { get; private set; }

        public SQLScript(IDatabaseAccessor accessor,  SQLTYPE type, Counter pcounter = null, Counter tcounter = null) : this(accessor, null, type, pcounter, tcounter)
        {
        }
        internal SQLScript(IDatabaseAccessor accessor, string id, SQLTYPE type, Counter pcounter, Counter tcounter):base(accessor, pcounter, tcounter)
        {
            ID = id;
            _list = new List<SQLItem>();
            TYPE = type;
            
        }

        public void AddItem(SQLItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            switch (TYPE)
            {
                case SQLTYPE.SELECT:
                    if (!TYPE_SELECT.Any(t => item.GetType().Equals(t) || item.GetType().IsSubclassOf(t)))
                    {
                        throw new ApplicationException(string.Format("SQLItem({0}) is illegal for the script({1})!", item.GetType().Name, TYPE));
                    }
                    _list.Add(item);
                    break;
                case SQLTYPE.INSERT:
                    if (!TYPE_INSERT.Any(t => item.GetType().Equals(t) || item.GetType().IsSubclassOf(t)))
                    {
                        throw new ApplicationException(string.Format("SQLItem({0}) is illegal for the script({1})!", item.GetType().Name, TYPE));
                    }
                    _list.Add(item);
                    break;
                case SQLTYPE.UPDATE:
                    if (!TYPE_UPDATE.Any(t => item.GetType().Equals(t) || item.GetType().IsSubclassOf(t)))
                    {
                        throw new ApplicationException(string.Format("SQLItem({0}) is illegal for the script({1})!", item.GetType().Name, TYPE));
                    }
                    _list.Add(item);
                    break;
                case SQLTYPE.DELETE:
                    if (!TYPE_DELETE.Any(t => item.GetType().Equals(t) || item.GetType().IsSubclassOf(t)))
                    {
                        throw new ApplicationException(string.Format("SQLItem({0}) is illegal for the script({1})!", item.GetType().Name, TYPE));
                    }
                    _list.Add(item);
                    break;
            }
        }
        public void AddItems(params SQLItem[] items)
        {
            if (items == null || items.Length <= 0) return;
            foreach (var item in items) AddItem(item);
        }

        

        
        public KeyValuePair<string, object>[] ExportParameters()
        {
            var list = new List<KeyValuePair<string, object>>();
            foreach (var item in _list)
            {
                var parameters = item.Parameters;
                if (parameters == null || parameters.Length <= 0) continue;
                list.AddRange(parameters);
            }
            return list.ToArray();
        }
        public DbParameter[] ExportParameters(IDatabaseAccessor accessor)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");

            var list = new List<DbParameter>();
            foreach (var item in _list)
            {
                var parameters = item.Parameters;
                if (parameters == null || parameters.Length <= 0) continue;
                parameters.ToList().ForEach(p =>
                {
                    list.Add(accessor.CreateParameter(p.Key, p.Value));
                });
            }
            return list.ToArray();
        }
        
        public override DbCommand ExportCommand()
        {
            var parameters = ExportParameters(accessor);
            return accessor.CreateCommand(SQLText, parameters);
        }

        protected override string BuildSQLText()
        {
            string text = null;
            switch (TYPE)
            {
                case SQLTYPE.SELECT:
                    text = BuildSQLText_SELECT();
                    break;
                case SQLTYPE.INSERT:
                    text = BuildSQLText_INSERT();
                    break;
                case SQLTYPE.UPDATE:
                    text = BuildSQLText_UPDATE();
                    break;
                case SQLTYPE.DELETE:
                    text = BuildSQLText_DELETE();
                    break;
            }

            return text;
        }

        private string BuildSQLText_SELECT()
        {
            if (_list.Count <= 0) return null;

            var buffer = new StringBuilder();
            //1.Parts:SELECT
            if (!_list.Any(itm => itm is SelectItem)) throw new ApplicationException("The script is missing select-item!");
            SelectItem select = null;
            _list.Where(itm => itm is SelectItem).ToList().ForEach(itm =>
            {
                select = select == null ? (SelectItem)itm : select + (SelectItem)itm;
            });
            buffer.AppendLine(select.Text);

            //2.Parts:FROM
            if (!_list.Any(itm => itm is FromItem)) throw new ApplicationException("The script is missing from-item!");
            if (_list.Count(itm => itm is FromItem) > 1) throw new ApplicationException("The script contains multiple from-items!");
            FromItem from = _list.First(itm => itm is FromItem) as FromItem;
            buffer.AppendLine(from.Text);

            //3.Parts:JOIN
            if (_list.Any(itm => itm is JoinItem))
            {
                _list.Where(itm => itm is JoinItem).ToList().ForEach(join =>
                {
                    buffer.AppendLine(join.Text);
                });
            }

            //4.Parts:WHERE
            //if (!_list.Any(itm => itm is WhereItem)) throw new ApplicationException("The script is missing where-item!");
            WhereItem where = null;
            if (_list.Any(itm => itm is WhereItem))
            {
                _list.Where(itm => itm is WhereItem).ToList().ForEach(itm =>
                {
                    where = where == null ? (WhereItem)itm : where + (WhereItem)itm;
                });
                buffer.AppendLine(where.Text);
            }

            return buffer.ToString();
        }
        private string BuildSQLText_INSERT()
        {
            if (_list.Count <= 0) return null;

            var buffer = new StringBuilder();
            //1.Parts:INSERT
            if (!_list.Any(itm => itm is InsertItem)) throw new ApplicationException("The script is missing insert-item!");
            buffer.AppendLine(_list.Where(itm => itm is InsertItem).First().Text.Trim());

            return buffer.ToString();
        }
        private string BuildSQLText_UPDATE()
        {
            if (_list.Count <= 0) return null;

            var buffer = new StringBuilder();
            //1.Parts:UPDATE
            if (!_list.Any(itm => itm is UpdateItem)) throw new ApplicationException("The script is missing update-item!");
            buffer.AppendLine(_list.Where(itm => itm is UpdateItem).First().Text.Trim());

            //2.Parts:WHERE
            WhereItem where = null;
            if (_list.Any(itm => itm is WhereItem))
            {
                _list.Where(itm => itm is WhereItem).ToList().ForEach(itm =>
                {
                    where = where == null ? (WhereItem)itm : where + (WhereItem)itm;
                });
                buffer.AppendLine(where.Text.Trim());
            }

            return buffer.ToString();
        }
        private string BuildSQLText_DELETE()
        {
            if (_list.Count <= 0) return null;

            var buffer = new StringBuilder();
            //1.Parts:DELETE
            if (!_list.Any(itm => itm is DeleteItem)) throw new ApplicationException("The script is missing delete-item!");
            buffer.AppendLine(_list.Where(itm => itm is DeleteItem).First().Text.Trim());

            //2.Parts:WHERE
            WhereItem where = null;
            if (_list.Any(itm => itm is WhereItem))
            {
                _list.Where(itm => itm is WhereItem).ToList().ForEach(itm =>
                {
                    where = where == null ? (WhereItem)itm : where + (WhereItem)itm;
                });
                buffer.AppendLine(where.Text.Trim());
            }

            return buffer.ToString();
        }
    }

    
}
