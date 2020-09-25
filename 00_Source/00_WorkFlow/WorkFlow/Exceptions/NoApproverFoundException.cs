using System;

namespace WorkFlow.Exceptions
{
    class NoApproverFoundException : WorkflowException
    {
        private const string message = "No approver found for the node!(NodeId:[{0}],NodeCode:[{1}],NodeName:[{2}])";
        public NoApproverFoundException(Guid nodeId, string nodeCode, string nodeName) : base(message, nodeId, nodeCode, nodeName)
        {
        }
    }
}
