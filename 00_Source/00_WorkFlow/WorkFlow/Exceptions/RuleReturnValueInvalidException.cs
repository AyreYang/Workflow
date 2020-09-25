namespace WorkFlow.Exceptions
{
    public class RuleReturnValueInvalidException<T> : WorkflowException
    {
        private const string message = "Return value is invalid!(T:[{0}],value:[{1}])";
        public RuleReturnValueInvalidException(object value) : base(message, typeof(T).ToString(), value == null ? "NULL" : value.GetType().ToString())
        {
        }
    }
}
