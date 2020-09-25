using System;

namespace WorkFlow.Exceptions
{
    public class WorkflowException : ApplicationException
    {
        public WorkflowException(string message, params object[] args) 
            : base(!string.IsNullOrWhiteSpace(message) && args != null && args.Length > 0 ? string.Format(message, args) : message)
        {
            
        }
    }
}
