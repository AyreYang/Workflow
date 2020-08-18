using System;
using System.Linq;
using WorkFlow.Components;
using WorkFlow.Interfaces;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Rules
{
    class OutputRules
    {
        public static OPResult DefaultRule0(IPNodeDetail[] details, INode[] nodes)
        {
            if (nodes == null || nodes.Length <= 0) throw new ArgumentNullException("nodes");

            var self = nodes[0];
            var children = nodes.Where(n => !n.Id.Equals(self.Id)).ToList();

            var result = new OPResult();
            if (details != null && details.Length > 0)
            {
                if (details.Any(d => d.Status == WorkFlow.Enums.NodeStatus.Rejected))
                {
                    result.Command = WorkFlow.Enums.OPCommand.Reject;
                    result.AddNodes(self.FetchSender());
                }
                else if (!details.Any(d => d.Status != WorkFlow.Enums.NodeStatus.Approved))
                {
                    result.Command = WorkFlow.Enums.OPCommand.Approve;
                    if (children.Count > 0) children.ForEach(n => result.AddNodes(n));
                }
            }
            return result;
        }
        public static OPResult DefaultRule1(IPNodeDetail[] details, INode[] nodes)
        {
            if (nodes == null || nodes.Length <= 0) throw new ArgumentNullException("nodes");

            var self = nodes[0];
            var children = nodes.Where(n => !n.Id.Equals(self.Id)).ToList();

            var result = new OPResult();
            if(details != null && details.Length > 0)
            {
                if(details.Any(d => d.Status == WorkFlow.Enums.NodeStatus.Rejected))
                {
                    result.Command = WorkFlow.Enums.OPCommand.Reject;
                    if(self.HasParents) self.FetchParents().ToList().ForEach(n => result.AddNodes(n));
                }
                else if(!details.Any(d => d.Status != WorkFlow.Enums.NodeStatus.Approved))
                {
                    result.Command = WorkFlow.Enums.OPCommand.Approve;
                    if (children.Count > 0) children.ForEach(n => result.AddNodes(n));
                }
            }
            return result;
        }
    }
}
