using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interfaces.Entities
{
    public interface ITask
    {
        Guid Id { get; }
        Guid? BizId { get; }
        Guid? Pid { get; }
        Guid? Nid { get; }

        Guid WorkFlowId { get; }
        Guid? NodeId { get; }
        string WFCode { get; }
        long Status { get; }
        string Comment { get; }
        string Parameter { get; }

        string CreatedBy { get; }
        DateTime CreatedOn { get; }
    }
}
