using Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Components;
using WorkFlow.Interfaces.Entities;
using WorkFlowEngine.Entities;
using WorkFlowEngine.Rules;

namespace WorkFlowEngine
{
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
}
