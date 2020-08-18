using System;
using System.Collections.Generic;
using WorkFlow.Components;
using WorkFlow.Enums;

namespace WorkFlow.Interfaces.Entities
{
    public interface IENode
    {
        Guid Id { get; }
        NodeType Type { get; }
        Guid[] Parents { get; }

        //IERule[] Rules { get; }

        Func<INode[], bool> RULE4Input { get; }
        Func<IPNodeDetail[], INode[], OPResult> RULE4Output { get; }
        Func<KeyValuePair<string, object>[], string[]> RULE4Approvers { get; }
    }
}
