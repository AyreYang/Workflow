using Database.Implements.SQLite;
using System;

namespace WorkFlowEngine
{
    public class WorkFlowFactory
    {
        private string _dbconnection = null;

        private static object _lock = new object();
        private static WorkFlowFactory _inst = null;
        public static WorkFlowFactory Inst
        {
            get
            {
                if(_inst == null)
                {
                    lock (_lock)
                    {
                        if (_inst == null) _inst = new WorkFlowFactory();
                    }
                }
                return _inst;
            }
        }

        private WorkFlowFactory()
        {
            _dbconnection = @"E:\01_Workspace\01_VS\05_WorkFlow\99_Temp\WorkFlow.s3db";
        }

        public WorkFlow.Components.WorkFlow CreateWorkFlow(Guid workflowId)
        {
            return new WorkFlow.Components.WorkFlow(workflowId, new WorkFlowContext(new DatabaseAccessor(_dbconnection)));
        }
    }
}
