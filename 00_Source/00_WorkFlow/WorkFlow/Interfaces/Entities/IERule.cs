using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interfaces.Entities
{
    public interface IERule
    {
        int RuleType { get; }
        int ProcType { get; }
        string Name { get; }
        string Content { get; }
    }
}
