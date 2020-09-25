using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Interfaces.Entities;

namespace WorkFlow.Components
{
    public class WorkFlow
    {
        public static string WFSENDER = "{WF-SENDER}";
        private Context Context { get; set; }
        public bool Ready { get; private set; }

        public Guid? Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }

        private List<Node> Nodes { get; set; }
        private Node StartNode { get; set; }
        private Node EndNode { get; set; }
        public WorkflowStatus WorkflowStatus
        {
            get
            {
                if (Context.Data == null) return WorkflowStatus.None;
                if (Context.Data.Status == (int)WorkflowStatus.Aborted) return WorkflowStatus.Aborted;

                var status = WorkflowStatus.None;
                switch (EndNode.NodeStatus)
                {
                    case NodeStatus.None:
                        status = WorkflowStatus.None;
                        break;
                    case NodeStatus.Approved:
                        status = WorkflowStatus.Completed;
                        break;
                    case NodeStatus.Error:
                        status = WorkflowStatus.Error;
                        break;
                    default:
                        status = WorkflowStatus.Processing;
                        break;
                }
                return status;
            }
        }
        public WorkflowStatus WorkflowStatus0
        {
            get
            {
                return Context.Data == null ? 
                    WorkflowStatus.None : 
                    Enum.IsDefined(typeof(WorkflowStatus), Context.Data.Status) ? 
                        (WorkflowStatus)Context.Data.Status : WorkflowStatus.Error;
            }
        }

        public WorkFlow(Guid workflowId, Context context)
        {
            if (Guid.Empty.Equals(workflowId)) throw new ArgumentException("Argument(workflowId) is empty!", "workflowId");
            if (context == null) throw new ArgumentNullException("context", "Argument(context) is null!");

            Id = null;
            Code = null;
            Name = null;
            Context = context;
            Nodes = new List<Node>();
            StartNode = null;
            EndNode = null;
            Ready = Initialize(workflowId);
        }

        private bool Initialize(Guid workflowId)
        {
            var result = false;
            try
            {
                var dworkflow = Context.FetchWorkflow(workflowId);
                if (dworkflow == null) throw new ApplicationException(string.Format("Workflow({0}) does not exsit!", dworkflow.ID));
                if (dworkflow.Nodes == null || dworkflow.Nodes.Length <= 0) throw new ApplicationException(string.Format("Workflow({0}:{1}) contains no nodes!", dworkflow.ID, dworkflow.Code));
                if (!dworkflow.Nodes.Any(n => n.Type.Equals((int)NodeType.Start))) throw new ApplicationException(string.Format("Workflow({0}:{1}) contains no sender node!", dworkflow.ID, dworkflow.Code));
                if (!dworkflow.Nodes.Any(n => n.Type.Equals((int)NodeType.End))) throw new ApplicationException(string.Format("Workflow({0}:{1}) contains no end node!", dworkflow.ID, dworkflow.Code));
                if (dworkflow.Nodes.Count(n => n.Type.Equals((int)NodeType.Start)) > 1) throw new ApplicationException(string.Format("Workflow({0}:{1}) contains muliple sender nodes!", dworkflow.ID, dworkflow.Code));
                if (dworkflow.Nodes.Count(n => n.Type.Equals((int)NodeType.End)) > 1) throw new ApplicationException(string.Format("Workflow({0}:{1}) contains muliple End nodes!", dworkflow.ID, dworkflow.Code));

                StartNode = CreateNode(dworkflow.Nodes.First(n => n.Type.Equals((int)NodeType.Start)));
                EndNode = CreateNode(dworkflow.Nodes.First(n => n.Type.Equals((int)NodeType.End)));
                Mapping(StartNode, dworkflow.Nodes.ToArray(), 1);

                Id = dworkflow.ID;
                Code = dworkflow.Code;
                Name = dworkflow.Name;

                result = true;
            }
            catch (Exception err)
            {
                StartNode = null;
                EndNode = null;
                Nodes.Clear();
                Context.Logging(err);
            }
            return result;
        }
        private void Mapping(Node node, IDNode[] nodes, int round)
        {
            nodes.Where(n => n.Parents != null && n.Parents.Length > 0 && n.Parents.Any(p => p.Equals(node.Id))).ToList()
                .ForEach(ent =>
                {
                    var exists = false;
                    var child = CreateNode(ent, out exists);
                    node.Append(child);

                    if (!exists)
                    {
                        Mapping(child, nodes, round + 1);
                    }
                });

            if (round == 1)
            {
                this.Nodes.Where(n => !n.HasChildren && !n.NodeType.Equals(NodeType.End)).ToList()
                    .ForEach(n =>
                    {
                        n.Append(this.EndNode);
                    });
            }
        }
        private Node CreateNode(IDNode dnode)
        {
            bool exists = false;
            return CreateNode(dnode, out exists);
        }
        private Node CreateNode(IDNode dnode, out bool exists)
        {
            exists = false;
            if (dnode == null) throw new ArgumentNullException("dnode");
            exists = this.Nodes.Any(n => n.Id.Equals(dnode.ID));
            var node = exists ? this.Nodes.First(n => n.Id.Equals(dnode.ID)) : new Node(dnode, Context);
            AddNode(node);
            return node;
        }
        private void AddNode(Node node)
        {
            if (node == null || this.Nodes.Any(n => n.Id.Equals(node))) return;
            this.Nodes.Add(node);
        }

        #region Public Methods
        public void ProcessTask(IRTask task)
        {
            if (task == null) throw new ArgumentNullException("task", "Argument(task) is null!");
            if (!this.Id.Equals(task.WorkflowID)) throw new ApplicationException(string.Format("Task data error, wrong workflow id(WorkflowID:{0}/{1})!", this.Id, task.WorkflowID));

            var user = task.CreatedBy;
            var action = task.Action == (int)TaskAction.Send ? task.InstID.HasValue ? TaskAction.Approve : TaskAction.Send : (TaskAction)task.Action;
            this.Context.LoadData(action, task);
            Context.SaveData(user, task.ID);

            if (WorkflowStatus == WorkflowStatus.Aborted || WorkflowStatus == WorkflowStatus.Error)
                throw new ApplicationException(string.Format("Workflow status error, wrong task action(WorkflowID:{0};WorkflowStatus:{1})!", this.Id, WorkflowStatus));

            if (action == TaskAction.Abort)
            {
                //更新流程状态
                Context.UpdateWorkflowStatus(user, WorkflowStatus.Aborted);
            }
            else
            {
                var node = (TaskAction)task.Action == TaskAction.Send ? StartNode :
                Nodes.FirstOrDefault(n => n.Id.Equals(Context.Nodes.First(rn => rn.ID.Equals(task.NodeID)).NodeID));
                if (node == null) throw new ApplicationException(string.Format("Task data error, no such node(RTNodeID:{0})!", task.NodeID));
                node.Process(action, task);

                //更新流程状态
                Context.UpdateWorkflowStatus(user, WorkflowStatus);
            }

            //提交到数据库
            Context.SaveData(user, null);
        }
        #endregion
    }

}
