using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interfaces.Entities
{
    public interface IPNode
    {
        Guid Id { get; }
        Guid NodeId { get; }
        Guid Pid { get; }
        int Status { get; }
        long Seq { get; }
        long RoundNO { get; }

        IList<IPNodeDetail> Details { get; }
    }
}
