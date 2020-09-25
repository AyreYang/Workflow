using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
