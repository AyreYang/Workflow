using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Interfaces;
using WorkFlow.Interfaces.Entities;

namespace WorkFlow.Components
{
    public class Node : INode
    {
        private Context Context { get; set; }
        internal IENode Entity { get; private set; }

        public Guid Id { get { return Entity.Id; } }
        public NodeType Type { get { return Entity.Type; } }

        
        protected IList<Node> Parents { get; private set; }
        public bool HasParents { get { return Parents != null && Parents.Count > 0; } }
        protected IList<Node> Children { get; private set; }
        public bool HasChildren { get { return Children != null && Children.Count > 0; } }
        

        internal IList<IPNodeDetail> Details
        {
            get
            {
                return Context.Data != null && Context.Data.Nodes != null 
                    && Context.Data.Nodes.Any(n => n.NodeId.Equals(this.Id) && n.Seq.Equals(this.SEQ)) ?
                    Context.Data.Nodes.First(n => n.NodeId.Equals(this.Id) && n.Seq.Equals(this.SEQ)).Details : null;
            }
        }
        internal bool HasDetails
        {
            get
            {
                var details = Details;
                return details != null && details.Count() > 0;
            }
        }

        public long SEQ
        {
            get
            {
                return Context.Data != null && Context.Data.Nodes != null && Context.Data.Nodes.Count(n => n.NodeId.Equals(this.Id)) > 0 ? 
                    Context.Data.Nodes.Where(n => n.NodeId.Equals(this.Id)).Max(n => n.Seq) : 0;
            }
        }
        public long RoundNO
        {
            get
            {
                return Context.Data != null && Context.Data.Nodes != null && Context.Data.Nodes.Count(n => n.NodeId.Equals(this.Id)) > 0 ?
                    Context.Data.Nodes.Where(n => n.NodeId.Equals(this.Id)).Max(n => n.RoundNO) : 0;
            }
        }
        public NodeStatus Status0
        {
            get
            {
                return Input.CanInput ? Output.Status : NodeStatus.Raw;
            }
        }
        public NodeStatus Status
        {
            get
            {
                var parent = HasParents ? this.Parents.Max(p => p.SEQ) : 0;
                return this.SEQ >= parent ? Status0 : NodeStatus.Raw;
            }
        }

        protected Input Input { get; private set; }
        protected Output Output { get; private set; }

        public Node(IENode ent, Context context)
        {
            if (ent == null) throw new ArgumentNullException("ent");
            if (context == null) throw new ArgumentNullException("context");
            if (Guid.Empty.Equals(ent.Id)) throw new ArgumentException("Node id is empty!", "ent.Id");

            Context = context;
            Entity = ent;
            Parents = new List<Node>();
            Children = new List<Node>();

            Input = new Input(this, Context, Parents);
            Output = new Output(this, Context, Children);
        }

        public void AppendTo(Node parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (!Parents.Any(p => p.Id.Equals(parent.Id))) Parents.Add(parent);
        }

        public void Append(Node child)
        {
            if (child == null) throw new ArgumentNullException("child");
            child.AppendTo(this);
            if (!Children.Any(c => c.Id.Equals(child.Id))) Children.Add(child);
        }

        internal void Process(ITask task)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (!Input.CanInput) return;

            var user = task.CreatedBy;
            Guid? nid = null;
            switch (task.Status)
            {
                case 0: //系统自动生成task, 作为新节点进入
                    nid = Context.CreateNewNodeData(user, Id, GetApprovers(user));
                    break;
                case 1: //系统自动生成task, 作为已存在节点进入
                //    Context.CreateExistedNodeData(user, Id, (int)this.SEQ, Entity.RULE4Approvers());
                //    break;
                case 2: //外部生成task, 通过
                case 3: //外部生成task, 拒绝
                    switch (this.Status)
                    {
                        case NodeStatus.None:
                        case NodeStatus.Processing:
                            
                            if(this.Status == NodeStatus.None) nid = Context.CreateNewNodeData(user, Id, GetApprovers(user));
                            if (!nid.HasValue) nid = task.Nid;
                            if (task.Status == 2) Context.ApproveNodeData(user, nid.Value, task.Comment, task.Parameter, task.BizId.HasValue ? new Guid[] { task.BizId .Value } : null);
                            if (task.Status == 3) Context.RejectNodeData(user, nid.Value, task.Comment, task.Parameter, task.BizId.HasValue ? new Guid[] { task.BizId.Value } : null);
                            break;
                        case NodeStatus.Raw:
                            throw new ApplicationException("没到该节点审批！");
                        case NodeStatus.Approved:
                        case NodeStatus.Rejected:
                            throw new ApplicationException("该节点已经完成审批！");
                        case NodeStatus.Error:
                            throw new ApplicationException("该节点状态目前有错误！");
                    }
                    break;

            }
            //更新节点状态
            Context.UpdateNodeStatus(user, nid.Value, this.Status);

            //发邮件提醒用户当前操作处理情况


            //当前节点是最终节点时退出
            if (Type == NodeType.End) return;

            //走到一下节点
            var result = Output.Go();
            switch (result.Command)
            {
                case OPCommand.approve:
                case OPCommand.reject:
                    if (!result.HasNodes) throw new ApplicationException(string.Format("Node({0}) is missing {1} node!", this.Id, result.Command.ToString().ToLower()));
                    foreach (var node in result.Nodes)
                    {
                        node.Process(Context.CreateTask(user, 0));
                    }
                    break;
            }
        }

        public INode[] FetchParents()
        {
            return HasParents ? Parents.Select(p => p as INode).ToArray() : null;
        }

        public INode FetchNode(Guid node)
        {
            return Context.Nodes.FirstOrDefault(n => n.Id.Equals(node)) as INode;
        }

        public INode FetchSender()
        {
            return Context.Nodes.FirstOrDefault(n => n.Type == NodeType.Sender) as INode;
        }

        private string[] GetApprovers(string user, params KeyValuePair<string, object>[] parameters)
        {
            var approvers = new List<string>();
            switch(Entity.Type)
            {
                case NodeType.Sender:
                    approvers.Add(Context.Data.OwnerID);
                    break;
                case NodeType.Approval:
                    var list = new List<KeyValuePair<string, object>>();
                    list.Add(new KeyValuePair<string, object>("NodeId", this.Id));
                    list.Add(new KeyValuePair<string, object>("UserId", user));
                    if (parameters != null && parameters.Length > 0) list.AddRange(parameters);
                    approvers.AddRange(this.Entity.RULE4Approvers(list.ToArray()));
                    break;
            }
            return approvers.ToArray();
        }

        
    }
}
