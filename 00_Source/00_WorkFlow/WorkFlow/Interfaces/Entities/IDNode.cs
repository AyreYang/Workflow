using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IDNode
    {
        Guid ID { get; }
        Guid WorkflowID { get; }
        string Code { get; }
        string Name { get; }
        int Type { get; }
        Guid[] Parents { get; }

        IDRule RL4Approvers { get; }
        IDRule RL4Input { get; }
        IDRule RL4Output { get; }
        IDRule RL4Status { get; }
        IDCallback CB4Approvers { get; }
        IDCallback CB4Input { get; }
        IDCallback CB4Output { get; }
        IDCallback CB4Notify { get; }


    }
}
