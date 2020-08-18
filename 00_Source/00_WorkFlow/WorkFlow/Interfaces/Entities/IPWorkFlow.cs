using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interfaces.Entities
{
    public interface IPWorkFlow : IDisposable
    {
        Guid Id { get; }
        Guid WorkFlowId { get; }
        int Status { get; }
        string OwnerID { get; }

        IList<IPNode> Nodes { get; }
    }
}
