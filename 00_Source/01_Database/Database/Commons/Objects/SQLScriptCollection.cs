using Database.Commons.Objects;
using Database.Commons.Objects.Enums;
using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Commons.Objects
{
    public class SQLScriptCollection : SQLContainer
    {
        private IList<SQLScript> _list { get; set; }

        public SQLScript[] Scripts
        {
            get { return _list.ToArray(); }
        }
        public bool IsEmpty { get { return _list.Count <= 0; } }

        public SQLScriptCollection(IDatabaseAccessor accessor):base(accessor, new Counter(), new Counter())
        {
            _list = new List<SQLScript>();
        }

        public void Clear()
        {
            _list.Clear();
        }

        public SQLScript NewSQLScript(string id, SQLTYPE type)
        {
            var script = new SQLScript(accessor, id, type, _pCounter, _tCounter);
            _list.Add(script);
            return script;
        }

        public DbCommand[] ExportCommands()
        {
            if (IsEmpty) throw new ApplicationException("SQLScriptCollection is empty!");
            var commands = new List<DbCommand>();
            foreach (var script in _list)
            {
                commands.Add(script.ExportCommand());
            }
            return commands.ToArray();
        }

        public override DbCommand ExportCommand()
        {
            if (IsEmpty) throw new ApplicationException("SQLScriptCollection is empty!");
            var parameters = new List<DbParameter>();
            foreach (var script in _list)
            {
                script.ExportParameters().ToList().ForEach(p =>
                {
                    if (!parameters.Any(param => param.ParameterName.Equals(p.Key))) parameters.Add(accessor.CreateParameter(p.Key, p.Value));
                });
            }
            return accessor.CreateCommand(SQLText, parameters.ToArray());
        }

        protected override string BuildSQLText()
        {
            var buffer = new StringBuilder();
            foreach (var script in _list)
            {
                buffer.AppendLine(script.SQLText.Trim() + ";");
            }
            return buffer.ToString();
        }
    }
}
