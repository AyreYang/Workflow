using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Database.Commons.Objects
{
    public struct STClause
    {
        public string Text { get; set; }
        public DbParameter[] Paramters { get; set; }
    }
    public class Clause
    {
        public enum Bracket
        {
            none,
            bracket0,
            bracket1,
            bracket2,
            bracket3
        }
        public enum Relation
        {
            AND, OR
        }

        internal IDatabaseAccessor accessor { get; private set; }

        private string text { get; set; }
        public string Text
        {
            get
            {
                return text;
            }
        }
        public string CommandText
        {
            get
            {
                string command = this.text;
                this.parameters.ToList().ForEach(key =>
                {
                    var pname = accessor.CreateParameterName(NakedName(key.Key).ToUpper());
                    command = command.Replace(key.Key, pname);
                });
                return command;
            }
        }
        private IList<KeyValuePair<string, object>> parameters { get; set; }
        public KeyValuePair<string, object>[] Parameters
        {
            get
            {
                return parameters.Select(p => new KeyValuePair<string, object>(p.Key, p.Value)).ToArray();
            }
        }
        public KeyValuePair<string, object>[] CommandParameters
        {
            get
            {
                return parameters.Select(p => new KeyValuePair<string, object>(accessor.CreateParameterName(NakedName(p.Key)), p.Value)).ToArray();
            }
        }

        public Clause(IDatabaseAccessor accessor, string text, params KeyValuePair<string, object>[] parameters)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException("text");

            this.accessor = accessor;
            this.text = text.Trim();
            this.parameters = new List<KeyValuePair<string, object>>();

            if (parameters != null) parameters.ToList().ForEach(p =>
            {
                AddParam(p.Key, p.Value);
            });
        }
        public Clause(IDatabaseAccessor accessor, string text) : this(accessor, text, null) { }

        private bool ParamExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            return this.parameters.Any(p => p.Key.ToUpper().Equals(name.Trim().ToUpper()));
        }
        public Clause AddParam(string name, object value)
        {
            if (!IsValidParamName(name)) return this;
            var pname = CoveredName(NakedName(name));
            if (!this.text.Contains(pname)) return this;
            if (ParamExists(pname)) return this;

            if (value == null
                || value is Guid
                || value is string
                || value is bool
                || value is DateTime
                || value is char
                || value is byte
                || value is byte[]
                || value is int
                || value is long
                || value is decimal)
            {
                this.parameters.Add(new KeyValuePair<string, object>(pname, value));
            }
            else
            {
                var values = new List<object>();
                if (value is List<string>) ((List<string>)value).ForEach(val => values.Add(val));
                if (value is string[]) ((string[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<bool>) ((List<bool>)value).ForEach(val => values.Add(val));
                if (value is bool[]) ((bool[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<DateTime>) ((List<DateTime>)value).ForEach(val => values.Add(val));
                if (value is DateTime[]) ((DateTime[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<int>) ((List<int>)value).ForEach(val => values.Add(val));
                if (value is int[]) ((int[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<long>) ((List<long>)value).ForEach(val => values.Add(val));
                if (value is long[]) ((long[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<decimal>) ((List<decimal>)value).ForEach(val => values.Add(val));
                if (value is decimal[]) ((decimal[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<char>) ((List<char>)value).ForEach(val => values.Add(val));
                if (value is char[]) ((char[])value).ToList().ForEach(val => values.Add(val));
                if (value is List<Guid>) ((List<Guid>)value).ForEach(val => values.Add(val));
                if (value is Guid[]) ((Guid[])value).ToList().ForEach(val => values.Add(val));

                var index = 0;
                var naked = NakedName(pname);
                var names = new List<string>();
                values.ToList().ForEach(val =>
                {
                    var ipname = string.Empty;
                    do
                    {
                        index++;
                        ipname = CoveredName(string.Format("{0}_{1}", naked, index));
                    } while (ParamExists(ipname));
                    names.Add(ipname);
                    this.parameters.Add(new KeyValuePair<string, object>(ipname, val));
                });
                if (names.Count > 0) this.text = this.text.Replace(pname, string.Join(",", names));
            }
            return this;
        }

        public static bool IsValidParamName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var real = NakedName(name);
            return System.Text.RegularExpressions.Regex.IsMatch(real, @"^[_a-zA-Z]{1}[_a-zA-Z0-9]*$");
        }
        private static string NakedName(string name)
        {
            return (name.StartsWith("{") && name.EndsWith("}")) ? name.Substring(1, name.Length - 2).Trim() : name.Trim();
        }
        private static string CoveredName(string name)
        {
            return "{" + (name ?? string.Empty) + "}";
        }

        /*
        public STClause Export(IDatabaseAccessor accessor)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            var temp = text;
            var list = new List<DbParameter>();
            this.parameters.Keys.ToList().ForEach(key =>
            {
                var naked = NakedName(key);
                var param = accessor.CreateParameter(naked, this.parameters[key]);
                temp = temp.Replace(key, param.ParameterName);
                list.Add(param);
            });
            return new STClause
            {
                Text = temp,
                Paramters = list.ToArray()
            };
        }

        internal void Export(out string text, out Dictionary<string, object> parameters)
        {
            text = this.text;
            parameters = this.parameters;
        }
        private void Import(string text, Dictionary<string, object> parameters)
        {
            this.text = text;
            this.parameters = parameters;
        }
        public Clause And(Clause clause, Bracket bracket = Bracket.none)
        {
            if (clause == null) return this;

            var result = Join(Relation.AND, this, clause, bracket);
            string text = null;
            Dictionary<string, object> parameters = null;
            result.Export(out text, out parameters);
            this.Import(text, parameters);
            return this;
        }
        public Clause Or(Clause clause, Bracket bracket = Bracket.none)
        {
            if (clause == null) return this;

            var result = Join(Relation.OR, this, clause, bracket);
            string text = null;
            Dictionary<string, object> parameters = null;
            result.Export(out text, out parameters);
            this.Import(text, parameters);
            return this;
        }
        */


        public static Clause operator +(Clause clause1, Clause clause2)
        {
            return Join(Relation.OR, clause1, clause2, Bracket.bracket0);
        }
        public static Clause operator *(Clause clause1, Clause clause2)
        {
            return Join(Relation.AND, clause1, clause2, Bracket.bracket0);
        }
        public Clause Append(params Clause[] clauses)
        {
            if (clauses != null || clauses.Length > 0)
            {
                var scripts = new List<string>();
                var parameters = new List<KeyValuePair<string, object>>(this.Parameters);
                scripts.Add(this.text);
                clauses.ToList().ForEach(clause =>
                {
                    var text = clause.Text;
                    clause.Parameters.ToList().ForEach(param =>
                    {
                        var key = param.Key;
                        var index = 0;
                        do
                        {
                            if (index > 0) key = CoveredName(NakedName(param.Key) + index.ToString());
                            index++;
                        } while (this.parameters.Any(p => p.Key.ToUpper().Equals(key)));
                        if (!param.Key.Equals(key)) text = (text ?? string.Empty).Replace(param.Key, key);
                        this.parameters.Add(new KeyValuePair<string, object>(key, param.Value));
                    });
                    scripts.Add(text);
                });

                this.text = string.Join(",", scripts);
            }

            return this;
        }


        private static Clause Join(Relation relation, Clause clause1, Clause clause2, Bracket bracket)
        {
            if (clause1 == null && clause2 == null) return null;
            if (clause1 == null) return clause2;
            if (clause2 == null) return clause1;

            var template = string.Empty;
            switch (bracket)
            {
                case Bracket.none:
                    template = "{0} {relation} {1}";
                    break;
                case Bracket.bracket0:
                    template = "({0}) {relation} ({1})";
                    break;
                case Bracket.bracket1:
                    template = "({0}) {relation} {1}";
                    break;
                case Bracket.bracket2:
                    template = "{0} {relation} ({1})";
                    break;
                case Bracket.bracket3:
                    template = "({0} {relation} {1})";
                    break;
            }
            template = template.Replace("{relation}", relation.ToString());

            var scripts = new List<string>();
            var parameters = new List<KeyValuePair<string, object>>();
            new List<Clause>() { clause1, clause2 }.ForEach(clause =>
            {
                var text = clause.Text;
                clause.Parameters.ToList().ForEach(param =>
                {
                    var key = param.Key;
                    var index = 0;
                    do
                    {
                        if (index > 0) key = CoveredName(NakedName(param.Key) + index.ToString());
                        index++;
                    } while (parameters.Any(p => p.Key.ToUpper().Equals(key)));
                    if (!param.Key.Equals(key)) text = (text ?? string.Empty).Replace(param.Key, key);
                    parameters.Add(new KeyValuePair<string, object>(key, param.Value));
                });
                scripts.Add(text);
            });

            return new Clause(clause1.accessor, string.Format(template, scripts[0], scripts[1]), parameters.ToArray());
        }

        public override string ToString()
        {
            var list = new List<string>();
            var template = "TEXT:{0}, PARAMS:({1})";
            if (parameters != null) parameters.ToList().ForEach(param => list.Add(string.Format("[{0}:{1}]", param.Key, param.Value == null ? "NULL" : param.Value.ToString())));
            return string.Format(template, this.text, string.Join(",", list));
        }

        public Clause Clone()
        {
            return new Clause(this.accessor, this.Text, this.Parameters);
        }
    }
}
