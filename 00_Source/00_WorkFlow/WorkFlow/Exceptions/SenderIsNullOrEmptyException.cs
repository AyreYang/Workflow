using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Exceptions
{
    public class SenderIsNullOrEmptyException : WorkflowException
    {
        private const string message = "Sender is null or empty!(InstId:[{0}],BizCode:[{1}]])";
        public SenderIsNullOrEmptyException(Guid instId, string bizCode) : base(message, instId, bizCode??"NULL")
        {
        }
    }
}
