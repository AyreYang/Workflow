using System;
using WorkFlow.Components;

namespace WorkFlow.Exceptions
{
    class NodeProcessException : WorkflowException
    {
        private const string _message = "Node process exceptions!(NodeId:[{0}],NodeCode:[{1}],NodeName:[{2}],Message:[{3}])";
        public NodeProcessException(Node node, string message) : base(_message, node.Id, node.Code, node.Name, message)
        {
        }
    }
}
