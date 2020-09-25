using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IRWorkflow
    {
        Guid ID { get; }
        Guid WorkflowID { get; }
        string BizCode { get; }
        int Status { get; }
        string OwnerID { get; }

        IRNode[] Nodes { get; }
    }
}
