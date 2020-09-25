using Database.Interfaces;
using System;
using WorkFlow.Components;
using WorkFlow.Interfaces.Entities;
using WorkFlowEntities.Entities;

namespace WorkFlowEngine
{
    internal class WorkFlowContext : Context
    {
        private IDatabaseAccessor accessor { get; set; }
        internal WorkFlowContext(IDatabaseAccessor accessor) : base()
        {
            if (accessor == null) throw new ArgumentNullException("accessor", "Argument(accessor) is null!");
            this.accessor = accessor;
        }

        public override void Logging(Exception err)
        {
            return;
        }

        protected override void CreateNodeData(string user, Guid nodeId, int seq, int round, string comment, string parameter, int status, params string[] approvers)
        {
            var workflow = Data as WF_RT_Workflow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");

            var id = Guid.NewGuid();
            var node = workflow.Nodes.NewEntity();
            node.ID = id;
            node.NodeID = nodeId;
            node.InstID = workflow.ID;
            node.SEQ = seq;
            node.RoundNO = round;
            node.CreatedBy = user;
            node.CreatedOn = DateTime.Now;
            node.LastModifiedBy = user;
            node.LastModifiedOn = DateTime.Now;

            if (approvers != null)
            {
                foreach (var approver in approvers)
                {
                    var detail = node.Details.NewEntity();
                    detail.ID = Guid.NewGuid();
                    detail.NodeID = node.ID;
                    detail.UserID = approver;
                    detail.Status = status;
                    detail.Enabled = true;
                    detail.CreatedBy = user;
                    detail.CreatedOn = DateTime.Now;
                    detail.LastModifiedBy = user;
                    detail.LastModifiedOn = DateTime.Now;
                }
            }
        }

        protected override IRTask CreateTaskData(string user, Guid workflowId, string bizCode, int action, string comment, string parameter)
        {
            var task = new WF_RT_Task();
            task.ID = Guid.NewGuid();
            task.DetailID = null;
            task.WorkflowID = workflowId;
            task.BizCode = bizCode;
            task.Action = action;
            task.Comment = comment;
            task.Parameter = parameter;
            task.CreatedBy = user;
            task.CreatedOn = DateTime.Now;
            return task;
        }

        protected override IRWorkflow FetchWorkflowData(Guid instId)
        {
            var workflow = new WF_RT_Workflow();
            workflow.ID = instId;
            workflow.Fresh(this.accessor);

            return workflow;
        }

        protected override IRWorkflow NewWorkflowData(IRTask task)
        {
            var workflow = new WF_RT_Workflow();
            workflow.ID = Guid.NewGuid();
            workflow.WorkflowID = task.WorkflowID;
            workflow.BizCode = task.BizCode;
            workflow.OwnerID = task.CreatedBy;
            workflow.CreatedBy = task.CreatedBy;
            workflow.CreatedOn = task.CreatedOn;
            workflow.ProcStatus = 0;
            workflow.TaskID = null;
            workflow.LastModifiedBy = task.CreatedBy;
            workflow.LastModifiedOn = task.CreatedOn;

            return workflow;
        }

        protected override void SaveWorkFlowData(string user, Guid? taskId)
        {
            var workflow = Data as WF_RT_Workflow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            workflow.ProcStatus = taskId.HasValue ? 1 : 0;
            workflow.TaskID = taskId;
            workflow.LastModifiedBy = user;
            workflow.LastModifiedOn = DateTime.Now;
            workflow.Save(this.accessor, true);
        }

        protected override void UpdateDetailData(string user, Guid nodeId, Guid detailId, int status, string comment, string parameter)
        {
            var workflow = Data as WF_RT_Workflow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            var node = workflow.Nodes.FirstOrDefault(n => n.ID.Equals(nodeId));
            if (node == null) throw new ApplicationException("No available node data!");
            var detail = node.Details.FirstOrDefault(d => d.ID.Equals(detailId));
            if (detail == null) throw new ApplicationException("No available detail data!");
            detail.Status = status;
            detail.Comment = comment;
            detail.Parameter = parameter;
            detail.LastModifiedBy = user;
            detail.LastModifiedOn = DateTime.Now;
        }

        protected override void UpdateNodeStatus(string user, Guid nodeId, int status)
        {
            var workflow = Data as WF_RT_Workflow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            var node = workflow.Nodes.FirstOrDefault(n => n.ID.Equals(nodeId));
            if (node == null) throw new ApplicationException(string.Format("Node({0}) doesn`t exist!", nodeId));
            node.Status = status;
            node.LastModifiedBy = user;
            node.LastModifiedOn = DateTime.Now;
        }

        protected override void UpdateWorkflowStatus(string user, int status)
        {
            var workflow = Data as WF_RT_Workflow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            workflow.Status = status;
            workflow.LastModifiedBy = user;
            workflow.LastModifiedOn = DateTime.Now;
        }

        protected override T FetchData<T>(Guid id)
        {
            object data = null;
            if(typeof(T).Equals(typeof(IDWorkflow)))
            {
                var workflow = new WF_DEF_Workflow();
                workflow.ID = id;
                workflow.Fresh(this.accessor);
                data = workflow;
            }

            return data != null ? (T)data : default(T);
        }
    }
    /*
    internal class WorkFlowContext : Context
    {
        private IDatabaseAccessor accessor { get; set; }

        internal WorkFlowContext(IDatabaseAccessor accessor, IEWorkFlow workflow) : base(workflow)
        {
            if (accessor == null) throw new ArgumentNullException("accessor");
            this.accessor = accessor;
        }
        public override void Logging(int type, string message, params object[] argms)
        {
            return;
        }

        protected override IPWorkFlow FetchData(ITask task)
        {
            if (task == null) throw new ArgumentNullException("task");

            var workflow = new WF_PRC_WorkFlow();
            if (!task.Pid.HasValue)
            {
                workflow.Id = Guid.NewGuid();
                workflow.WorkFlowId = this.WorkFlowId;
                workflow.WFCode = task.WFCode;
                workflow.OwnerID = task.CreatedBy;
                workflow.CreatedBy = task.CreatedBy;
                workflow.CreatedOn = task.CreatedOn;
            }
            else
            {
                workflow.Id = task.Pid.Value;
                workflow.Fresh(this.accessor);
            }
            workflow.ProcStatus = true;
            workflow.TaskId = task.Id;
            workflow.LastModifiedBy = task.CreatedBy;
            workflow.LastModifiedOn = task.CreatedOn;

            workflow.Save(this.accessor, true);

            return workflow;
        }

        protected override Guid CreateNodeData(string user, Guid nid, int seq, int round, params string[] approvers)
        {
            var workflow = Data as WF_PRC_WorkFlow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");

            var id = Guid.NewGuid();
            var node = workflow.Nodes.NewEntity();
            node.Id = id;
            node.NodeId = nid;
            node.Pid = workflow.Id;
            node.SEQ = seq;
            node.RoundNO = round;
            node.CreatedBy = user;
            node.CreatedOn = DateTime.Now;
            node.LastModifiedBy = user;
            node.LastModifiedOn = DateTime.Now;

            if (approvers != null)
            {
                foreach(var approver in approvers)
                {
                    var detail = node.Details.NewEntity();
                    detail.Id = Guid.NewGuid();
                    detail.Nid = node.Id;
                    detail.UserID = approver;
                    detail.Enabled = true;
                    detail.CreatedBy = user;
                    detail.CreatedOn = DateTime.Now;
                    detail.LastModifiedBy = user;
                    detail.LastModifiedOn = DateTime.Now;
                }
            }
            return id;
        }
        protected override void UpdateNodeData(string user, Guid nid, int status, string comment, string parameter, params Guid[] bizId)
        {
            var workflow = Data as WF_PRC_WorkFlow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            var node = workflow.Nodes.FirstOrDefault(n => n.Id.Equals(nid));
            if(node == null) throw new ApplicationException(string.Format("Missing node data({0})!", nid));
            var list = new List<WF_PRC_NodeDetail>();
            if (bizId == null || bizId.Length <= 0)
            {
                list.AddRange(node.Details.Entities);
            }
            else
            {
                list.AddRange(node.Details.Entities.Where(e => bizId.Any(b => b.Equals(e.Id))));
            }
            list.ForEach(e =>
            {
                e.Status = status;
                e.Comment = comment;
                e.Parameter = parameter;
                e.LastModifiedBy = user;
                e.LastModifiedOn = DateTime.Now;
            });
        }
        protected override ITask CreateTask(string user, Guid workflow, int status)
        {
            var task = new WF_VW_TASK();
            task.WorkFlowID = workflow.ToString();
            task.Status = status;
            task.CreatedBy = user;
            task.CreatedOn = DateTime.Now;

            return task;
        }

        protected override void SaveWorkFlowData()
        {
            var workflow = Data as WF_PRC_WorkFlow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            workflow.Save(this.accessor);
        }

        protected override void UpdateNodeStatus(string user, Guid nid, int status)
        {
            var workflow = Data as WF_PRC_WorkFlow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            var node = workflow.Nodes.FirstOrDefault(n => n.Id.Equals(nid));
            if (node == null) throw new ApplicationException(string.Format("Node({0}) doesn`t exist!", nid));
            node.Status = status;
            node.LastModifiedBy = user;
            node.LastModifiedOn = DateTime.Now;
        }

        protected override void UpdateWorkflowStatus(string user, int status)
        {
            var workflow = Data as WF_PRC_WorkFlow;
            if (workflow == null) throw new ApplicationException("No available workflow data!");
            workflow.Status = status;
            workflow.LastModifiedBy = user;
            workflow.LastModifiedOn = DateTime.Now;
        }

        public override void Dispose()
        {
            RuleFactory.Inst.ReleaseAssembly(this.WorkFlowId);
        }
    }
    */
}
