using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System;

namespace DataBase.common.objects
{
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

        private string text { get; set; }
        private Dictionary<string, object> parameters { get; set; }

        public Clause(string clause)
        {
            this.text = (clause ?? string.Empty).Trim();
            this.parameters = new Dictionary<string, object>();
        }

        public Clause AddParam(string name, object value)
        {
            if (!IsValidParamName(name)) return this;
            var pname = CoveredName(NakedName(name));
            if (!this.text.Contains(pname)) return this;
            if (this.parameters.ContainsKey(pname)) return this;

            if (value == null
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
                this.parameters.Add(pname, value);
            }
            else
            {
                var values = new List<object>();
                if(value is List<string>)((List<string>)value).ForEach(val=>values.Add(val));
                if(value is string[])((string[])value).ToList().ForEach(val=>values.Add(val));
                if(value is List<bool>)((List<bool>)value).ForEach(val=>values.Add(val));
                if(value is bool[])((bool[])value).ToList().ForEach(val=>values.Add(val));
                if(value is List<DateTime>)((List<DateTime>)value).ForEach(val=>values.Add(val));
                if(value is DateTime[])((DateTime[])value).ToList().ForEach(val=>values.Add(val));
                if(value is List<int>)((List<int>)value).ForEach(val=>values.Add(val));
                if(value is int[])((int[])value).ToList().ForEach(val=>values.Add(val));
                if(value is List<long>)((List<long>)value).ForEach(val=>values.Add(val));
                if(value is long[])((long[])value).ToList().ForEach(val=>values.Add(val));
                if(value is List<decimal>)((List<decimal>)value).ForEach(val=>values.Add(val));
                if(value is decimal[])((decimal[])value).ToList().ForEach(val=>values.Add(val));
                if(value is List<char>)((List<char>)value).ForEach(val=>values.Add(val));
                if(value is char[])((char[])value).ToList().ForEach(val=>values.Add(val));

                var index = 0;
                var naked = NakedName(pname);
                var names = new List<string>();
                values.ToList().ForEach(val =>
                {
                    var ipname = string.Empty;
                    do
                    {
                        index++;
                        ipname = CoveredName(string.Format("{0}{1}", naked, index));
                    } while (this.parameters.ContainsKey(ipname));
                    names.Add(ipname);
                    this.parameters.Add(ipname, val);
                });
                if (names.Count > 0) this.text = this.text.Replace(pname, string.Join(",", names));
            }
            return this;
        }

        public static bool IsValidParamName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var real = NakedName(name);
            return System.Text.RegularExpressions.Regex.IsMatch(real, @"^[a-zA-Z]{1}[a-zA-Z0-9]*$");
        }
        private static string NakedName(string name)
        {
            return (name.StartsWith("{") && name.EndsWith("}")) ? name.Substring(1, name.Length - 2).Trim() : name.Trim();
        }
        private static string CoveredName(string name)
        {
            return "{" + (name ?? string.Empty) + "}";
        }

        public void Export(DatabaseCore accessor, out string clause, out List<DbParameter> parameters)
        {
            clause = null;parameters = null;
            if(accessor == null)return;
            var temp = text;
            var list = new List<DbParameter>();
            this.parameters.Keys.ToList().ForEach(key =>
            {
                var naked = NakedName(key);
                var param = accessor.CreateParameter(naked, this.parameters[key]);
                temp = temp.Replace(key, param.ParameterName);
                list.Add(param);
            });
            clause = temp;
            parameters = list;
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

        public static Clause operator +(Clause clause1, Clause clause2)
        {
            return Join(Relation.OR, clause1, clause2, Bracket.bracket0);
        }
        public static Clause operator *(Clause clause1, Clause clause2)
        {
            return Join(Relation.AND, clause1, clause2, Bracket.bracket0);
        }
        public static Clause Join(Relation relation, Clause[] clauses)
        {
            if (clauses == null || clauses.Length <= 0) return null;

            var scripts = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (clauses != null && clauses.Length > 0) clauses.ToList().ForEach(clause =>
            {
                string text = null;
                Dictionary<string, object> list = null;
                if (clause != null) clause.Export(out text, out list);
                if (list != null && list.Count > 0) list.Keys.ToList().ForEach(key1 =>
                {
                    var key = key1;
                    var index = 0;
                    do
                    {
                        if (index > 0) key = CoveredName(NakedName(key1) + index.ToString());
                        index++;
                    } while (parameters.ContainsKey(key));
                    if (!key1.Equals(key)) text = (text ?? string.Empty).Replace(key1, key);
                    parameters.Add(key, list[key1]);
                });
                scripts.Add(text);
            });

            var sb = new StringBuilder();
            scripts.ForEach(script =>
            {
                if (sb.Length > 0) sb.AppendLine(relation.ToString());
                sb.AppendLine(string.Format("({0})", script));
            });

            var result = new Clause(sb.ToString());
            parameters.Keys.ToList().ForEach(key => result.AddParam(key, parameters[key]));
            return result;
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
            var parameters = new Dictionary<string, object>();
            new List<Clause>() { clause1, clause2 }.ForEach(clause =>
            {
                string text = null;
                Dictionary<string, object> list = null;
                if (clause != null) clause.Export(out text, out list);
                if (list != null && list.Count > 0) list.Keys.ToList().ForEach(key1 =>
                {
                    var key = key1;
                    var index = 0;
                    do
                    {
                        if (index > 0) key = CoveredName(NakedName(key1) + index.ToString());
                        index++;
                    } while (parameters.ContainsKey(key));
                    if (!key1.Equals(key)) text = (text ?? string.Empty).Replace(key1, key);
                    parameters.Add(key, list[key1]);
                });
                scripts.Add(text);
            });

            var script = string.Format(template, scripts[0], scripts[1]);
            var result = new Clause(script);
            parameters.Keys.ToList().ForEach(key => result.AddParam(key, parameters[key]));
            return result;
        }

        public override string ToString()
        {
            var list = new List<string>();
            var template = "TEXT:{0}, PARAMS:({1})";
            if (parameters != null) parameters.Keys.ToList().ForEach(key => list.Add(string.Format("[{0}:{1}]", key, parameters[key] == null ? "NULL" : parameters[key].ToString())));
            return string.Format(template, this.text, string.Join(",", list));
        }
    }
}
