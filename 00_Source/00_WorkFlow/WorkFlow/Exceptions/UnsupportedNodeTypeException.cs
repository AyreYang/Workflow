using System;

namespace WorkFlow.Exceptions
{
    public class UnsupportedNodeTypeException : WorkflowException
    {
        private const string message = "Unsupported node type!(NodeId:[{0}],NodeCode:[{1}],NodeName:[{2}],NodeType:[{3}])";
        public UnsupportedNodeTypeException(Guid nodeId, string nodeCode, string nodeName, int nodeType) : base(message, nodeId, nodeCode, nodeName, nodeType)
        {
        }
    }
}
