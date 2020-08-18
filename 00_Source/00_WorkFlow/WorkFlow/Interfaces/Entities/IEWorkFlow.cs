using System;
using System.Collections.Generic;

namespace WorkFlow.Interfaces.Entities
{
    public interface IEWorkFlow
    {
        Guid Id { get; }
        string Name { get; }

        IENode[] Nodes { get; }
    }
}
