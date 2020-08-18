using Database.Interfaces;
using System;
using WorkFlowEngine.Entities;

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
            var task = new WF_TSK_TASK();
            task.Id = Guid.NewGuid();
            task.BizId = null;
            task.WorkFlowId = WorkFlowId.Value;
            task.WFCode = code.Trim().ToUpper();
            task.Status = 2;
            task.Comment = "发起";
            task.Parameter = parameters;
            task.CreatedBy = user.Trim().ToUpper();
            task.CreatedOn = DateTime.Now;

            task.Save(Accessor);

            return task.Id;
        }
        public virtual Guid CreateApprovalTask(string user, Guid bizId, int status, string comment, string parameters = null)
        {
            if (!WorkFlowId.HasValue) throw new ApplicationException("Workflow ID is null!");
            var task = new WF_TSK_TASK();
            task.Id = Guid.NewGuid();
            task.BizId = bizId;
            task.WorkFlowId = WorkFlowId.Value;
            task.WFCode = null;
            task.Status = status;
            task.Comment = comment;
            task.Parameter = parameters;
            task.CreatedBy = user.Trim().ToUpper();
            task.CreatedOn = DateTime.Now;

            task.Save(Accessor);

            return task.Id;
        }
        public virtual void DeleteTask(Guid id)
        {
            var task = new WF_TSK_TASK();
            task.Id = id;
            task.Delete(Accessor);
        }
    }
}
