using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IDRule
    {
        Guid ID { get; }
        Guid NodeID { get; }
        string Code { get; }
        string Name { get; }
        int Type { get; }
        int Flag { get; }
        string Script { get; }
    }
}
