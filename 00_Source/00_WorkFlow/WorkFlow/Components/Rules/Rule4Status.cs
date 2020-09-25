using System;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Exceptions;
using WorkFlow.Interfaces.Entities;

namespace WorkFlow.Components.Rules
{
    public class Rule4Status : Rule
    {
        private const string Template = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public class Express
            {
                public int Test(dynamic[] details, dynamic parameter)
                {
                    {body}
                }
            }";

        public Rule4Status(string script) : base(script)
        {
            this.SetFlag(99);
        }
        public Rule4Status(int flag) : base()
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

        public int GetStatus(IRDetail[] details, dynamic parameter)
        {
            var result = this.Test(new object[] { details, parameter });
            if (result == null || !(result is int)) throw new RuleReturnValueInvalidException<int>(result);
            var status = (int)result;
            if (!Enum.IsDefined(typeof(NodeStatus), status)) throw new ApplicationException(string.Format("Return status is invalid!(status:{0})", status));
            //if (!Enum.GetValues(typeof(NodeStatus)).Cast<int>().Contains(status)) 
            return status;
        }

        public int Rule0(IRDetail[] details, dynamic parameter)
        {
            int status = (int)NodeStatus.None;
            if (details != null && details.Length > 0)
            {
                if (details.Any(d => d.Status == (int)DetailStatus.Rejected))
                {
                    status = (int)NodeStatus.Rejected;
                }
                else
                {
                    if (details.Count(d => d.Status == (int)DetailStatus.Approved) == details.Length)
                    {
                        status = (int)NodeStatus.Approved;
                    }
                    else
                    {
                        status = (int)NodeStatus.Processing;
                    }
                }
            }
            return status;
        }
    }
}
