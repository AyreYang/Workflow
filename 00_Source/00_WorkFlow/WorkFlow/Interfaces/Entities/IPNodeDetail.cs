using System;
using WorkFlow.Enums;

namespace WorkFlow.Interfaces.Entities
{
    public interface IPNodeDetail
    {
        Guid Id { get; }
        Guid Nid { get; }
        NodeStatus Status { get; }
    }
}
