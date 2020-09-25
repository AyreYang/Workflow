using System;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Exceptions;
using WorkFlow.Interfaces;

namespace WorkFlow.Components.Rules
{
    public class Rule4Input : Rule
    {
        private const string Template = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public class Express
            {
                public bool Test(dynamic[] nodes, dynamic parameter)
                {
                    {body}
                }
            }";

        public Rule4Input(string script) : base(script)
        {
            this.SetFlag(99);
        }
        public Rule4Input(int flag) : base()
        {
            this.SetFlag(flag);
            switch (flag)
            {
                case 0:
                    this.Initialize(this, this.GetType().GetMethod("Rule0"));
                    break;
                case 1:
                    this.Initialize(this, this.GetType().GetMethod("Rule1"));
                    break;
                default:
                    throw new ArgumentException(string.Format("flag({0}) is unsupported!", flag), "flag");
            }
        }

        protected override string BuildSource(string script, out string className, out string methodName)
        {
            className = "Express";
            methodName = "Test";
            return Template.Replace("{body}", script);
        }

        public bool CanInput(Node[] nodes, dynamic parameter)
        {
            //if (nodes == null || nodes.Length == 0) throw new ArgumentNullException("nodes", "Argument(nodes) is null or empty!");
            var result = this.Test(new object[] { nodes, parameter });
            if (result == null || !(result is bool)) throw new RuleReturnValueInvalidException<bool>(result);
            return (bool)result;
        }

        public bool Rule0(Node[] nodes, dynamic parameter)
        {
            return nodes.Count(n => n.Status == (int)NodeStatus.Approved) == nodes.Length;
        }
        public bool Rule1(Node[] nodes, dynamic parameter)
        {
            return nodes.Any(n => n.Status == (int)NodeStatus.Approved);
        }
    }
}
