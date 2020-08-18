using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Enums
{
    public enum NodeType
    {
        UNKNOWN = -1,
        Sender = 0,
        Approval = 1,
        End = 2
    }
    public enum RuleType
    {
        Rule4Approvers = 0,
        Rule4Input = 1,
        Rule4Output = 2
    }

    public enum OPCommand
    {
        none = 0,
        approve = 2,
        reject = 3
    }

    public enum WorkflowStatus
    {
        Raw = -1,
        None = 0,
        Processing = 1,
        Completed = 2,
        Aborted = 4,
        Error = 99
    }

    public enum NodeStatus
    {
        Raw = -1,
        None = 0,
        Processing = 1,
        Approved = 2,
        Rejected = 3,
        Aborted = 4,
        Pending = 5,
        Error = 99
    }
}
