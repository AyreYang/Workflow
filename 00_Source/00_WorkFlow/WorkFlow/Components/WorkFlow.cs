using System;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Interfaces.Entities;

namespace WorkFlow.Components
{
    public class WorkFlow
    {
        public Guid Id { get { return Context.WorkFlowId; } }
        public string Name { get { return Context.WorkFlowName; } }

        public bool Ready { get; private set; }
        public WorkflowStatus Status
        {
            get
            {
                return !Ready ? WorkflowStatus.Raw : End.Status == NodeStatus.Approved ? WorkflowStatus.Completed : WorkflowStatus.Processing;
            }
        }

        private Node Sender { get; set; }
        private Node End { get; set; }
        private Context Context { get; set; }

        public WorkFlow(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.Context = context;
            Ready = Initialize();
        }


        #region Public Methods
        public void ProcessTask(ITask task)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (!this.Id.Equals(task.WorkFlowId)) throw new ApplicationException(string.Format("Task data error, wrong workflow id({0})!", task.WorkFlowId));

            var user = task.CreatedBy;
            this.Context.LoadData(task);

            Node node = null;

            if (!task.NodeId.HasValue)
            {
                //发起
                node = Sender;
            }
            else
            {
                //审批
                node = Context.Nodes.FirstOrDefault(n => n.Id.Equals(task.NodeId));
                if (node == null) throw new ApplicationException(string.Format("Task data error, no such node({0})!", task.NodeId));
            }
            node.Process(task);

            //更新流程状态
            Context.UpdateWorkflowStatus(user, this.Status);

            //提交到数据库
            Context.SaveData();
        }
        #endregion
        private bool Initialize()
        {
            var result = false;
            try
            {
                var ent = Context.Entity;

                if (ent.Nodes == null || ent.Nodes.Length <= 0) throw new ApplicationException("WorkFlow contains no nodes!");
                if (!ent.Nodes.Any(n => n.Type.Equals(NodeType.Sender))) throw new ApplicationException(string.Format("WorkFlow({0}) contains no sender node!", this.Name));
                if (!ent.Nodes.Any(n => n.Type.Equals(NodeType.End))) throw new ApplicationException(string.Format("WorkFlow({0}) contains no end node!", this.Name));
                if (ent.Nodes.Count(n => n.Type.Equals(NodeType.Sender)) > 1) throw new ApplicationException(string.Format("WorkFlow({0}) contains muliple sender nodes!", this.Name));
                if (ent.Nodes.Count(n => n.Type.Equals(NodeType.End)) > 1) throw new ApplicationException(string.Format("WorkFlow({0}) contains muliple End nodes!", this.Name));

                Sender = CreateNode(ent.Nodes.First(n => n.Type.Equals(NodeType.Sender)));
                End = CreateNode(ent.Nodes.First(n => n.Type.Equals(NodeType.End)));
                Build(Sender, ent.Nodes.ToArray(), 1);

                result = true;
            }
            catch (Exception err)
            {
                Context.LoggingError(err);
            }
            return result;
        }
        private void Build(Node node, IENode[] nodes, int round)
        {
            nodes.Where(n => n.Parents != null && n.Parents.Length > 0 && n.Parents.Any(p => p.Equals(node.Id))).ToList()
                .ForEach(ent =>
            {
                var exists = false;
                var child = CreateNode(ent, out exists);
                node.Append(child);

                if (!exists)
                {
                    Build(child, nodes, round + 1);
                }
            });

            if (round == 1)
            {
                Context.Nodes.Where(n => !n.HasChildren && !n.Type.Equals(NodeType.End)).ToList()
                    .ForEach(n =>
                    {
                        n.Append(this.End);
                    });
            }
        }

        private Node CreateNode(IENode ent)
        {
            bool exists = false;
            return CreateNode(ent, out exists);
        }
        private Node CreateNode(IENode ent, out bool exists)
        {
            exists = false;
            if (ent == null) throw new ArgumentNullException("ent");
            exists = Context.Nodes.Any(n => n.Id.Equals(ent.Id));
            var node = exists ? Context.Nodes.First(n => n.Id.Equals(ent.Id)) : new Node(ent, Context);
            AddNode(node);
            return node;
        }
        private void AddNode(Node node)
        {
            if (node == null || Context.Nodes.Any(n => n.Id.Equals(node))) return;
            Context.Nodes.Add(node);
        }
    }

    
}
