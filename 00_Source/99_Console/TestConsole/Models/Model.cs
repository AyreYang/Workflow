using Database.Interfaces;
using System;
using WorkFlowEntities.Entities;

namespace TestConsole.Models
{
    public abstract class Model
    {
        protected IDatabaseAccessor Accessor { get; private set; }
        protected Guid? WorkFlowId { get; private set; }

        protected Model(IDatabaseAccessor accessor, Guid? wid)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            Accessor = accessor;
            WorkFlowId = wid;
        }
        public Model(IDatabaseAccessor accessor) :this(accessor, null)
        {
        }
        public abstract void CreateWorkFlow();

        public virtual Guid CreateSendTask(string user, string code, string parameters = null)
        {
            if (!WorkFlowId.HasValue) throw new ApplicationException("Workflow ID is null!");
            if(string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException("code");
            var task = new WF_RT_Task();
            task.ID = Guid.NewGuid();
            task.DetailID = null;
            task.WorkflowID = WorkFlowId.Value;
            task.BizCode = code.Trim().ToUpper();
            task.Action = 1;
            task.Comment = "发起";
            task.Parameter = parameters;
            task.CreatedBy = user.Trim().ToUpper();
            task.CreatedOn = DateTime.Now;

            task.Save(Accessor);

            return task.ID;
        }
        public virtual Guid CreateApprovalTask(string user, Guid bizId, int action, string comment, string parameters = null)
        {
            if (!WorkFlowId.HasValue) throw new ApplicationException("Workflow ID is null!");
            var task = new WF_RT_Task();
            task.ID = Guid.NewGuid();
            task.DetailID = bizId;
            task.WorkflowID = WorkFlowId.Value;
            task.BizCode = null;
            task.Action = action;
            task.Comment = comment;
            task.Parameter = parameters;
            task.CreatedBy = user.Trim().ToUpper();
            task.CreatedOn = DateTime.Now;

            task.Save(Accessor);

            return task.ID;
        }
        public virtual void DeleteTask(Guid id)
        {
            var task = new WF_RT_Task();
            task.ID = id;
            task.Delete(Accessor);
        }

        public WF_DEF_Workflow GetWorkflow()
        {
            var workflow = new WF_DEF_Workflow();
            workflow.ID = this.WorkFlowId.Value;

            workflow.Fresh(this.Accessor);
            return workflow;
        }
    }
}
