using System;
using WorkFlow.Exceptions;

namespace WorkFlow.Components.Rules
{
    public class Rule4Approvers : Rule
    {
        private const string Template = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public class Express
            {
                public string[] Test(dynamic parameter)
                {
                    {body}
                }
            }";

        public Rule4Approvers(string script) : base(script)
        {
            this.SetFlag(99);
        }
        public Rule4Approvers(int flag) : base()
        {
            this.SetFlag(flag);
            switch (flag)
            {
                case 0:
                    this.Initialize(this, this.GetType().GetMethod("Rule0"));
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

        public string[] GetApprovers(dynamic parameter)
        {
            var result = this.Test(new object[] { parameter });
            if (result == null || !(result is string[])) throw new RuleReturnValueInvalidException<string[]>(result);

            string[] approvers = (string[])result;
            return (this.Flag == 99 && approvers.Length <= 0) ? null : approvers;
        }

        public string[] Rule0(dynamic parameter)
        {
            return new string[0];
        }
    }
}
