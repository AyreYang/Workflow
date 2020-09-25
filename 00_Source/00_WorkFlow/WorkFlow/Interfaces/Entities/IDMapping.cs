using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IDMapping
    {
        Guid ID { get; }
        Guid WorkflowID { get; }
        Guid NodeID { get; }
        Guid ParentID { get; }
    }
}
