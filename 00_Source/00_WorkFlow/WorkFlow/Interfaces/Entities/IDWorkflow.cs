using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interfaces.Entities
{
    public interface IDWorkflow
    {
        Guid ID { get; }
        string Code { get;}
        string Name { get; }

        IDNode[] Nodes { get; }

        IDCallback CB4Created { get; }
        IDCallback CB4Finished { get; }
        IDCallback CB4Error { get; }

    }
}
