using System.Linq;
using WorkFlow.Interfaces;

namespace WorkFlowEngine.Rules
{
    class InputRules
    {
        public static bool DefaultRule(INode[] nodes)
        {
            return nodes != null && nodes.Length > 0 ? nodes.Any(n => n.Status != WorkFlow.Enums.NodeStatus.Approved) ? false : true : true;
        }
    }
}
