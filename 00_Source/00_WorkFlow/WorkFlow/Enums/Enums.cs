namespace WorkFlow.Enums
{
    public enum WorkflowStatus
    {
        None = 0,
        Processing = 1,
        Completed = 2,
        Aborted = 3,
        Error = 4
    }
    public enum NodeType
    {
        UNKNOWN = -1,
        Start = 0,
        Normal = 1, //with approvers
        Control = 2,    //without approvers
        End = 9
    }
    public enum NodeStatus
    {
        Disable = -1,
        None = 0,
        Processing = 1,
        Approved = 2,
        Rejected = 3,
        //Pending = 5,
        Error = 99
    }
    public enum DetailStatus
    {
        None = 0,
        Processing = 1,
        Approved = 2,
        Rejected = 3
    }

    public enum TaskAction
    {
        None = 0,   //自动生成task专用
        Send = 1,
        Approve = 2,
        Reject = 3,
        Abort = 4
    }

    public enum RuleType
    {
        Rule4Approvers = 0,
        Rule4Input = 1,
        Rule4Output = 2,
        Rule4Status = 3
    }

    public enum CallbackType
    {
        Callback4Approvers = 0,
        Callback4Input = 1,
        Callback4Output = 2,
        Callback4Notify = 3
    }
}
