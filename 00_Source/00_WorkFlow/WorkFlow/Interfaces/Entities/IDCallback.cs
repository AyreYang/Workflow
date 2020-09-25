using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IDCallback
    {
        Guid ID { get; }
        Guid WorkflowID { get; }
        string Code { get; }
        string Name { get; }
        string Url { get; }
    }
}
