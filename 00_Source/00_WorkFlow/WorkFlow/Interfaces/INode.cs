using System;
using WorkFlow.Enums;

namespace WorkFlow.Interfaces
{
    public interface INode
    {
        Guid Id { get; }
        NodeStatus Status { get; }
        bool HasParents { get; }
        bool HasChildren { get; }

        INode[] FetchParents();
        INode FetchNode(Guid node);
        INode FetchSender();
    }
}
