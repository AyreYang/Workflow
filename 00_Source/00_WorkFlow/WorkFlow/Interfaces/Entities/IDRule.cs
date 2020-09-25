using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
