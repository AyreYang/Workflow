using System;
using WorkFlow.Components;

namespace WorkFlow.Exceptions
{
    class NoOutputNodesFoundException : WorkflowException
    {
        private const string _message = "No output nodes found for the node!(NodeId:[{0}],NodeCode:[{1}],NodeName:[{2}],NodeStatus:[3])";
        public NoOutputNodesFoundException(Node node) : base(_message, node.Id, node.Code, node.Name, node.NodeStatus.ToString())
        {
        }
    }
}
