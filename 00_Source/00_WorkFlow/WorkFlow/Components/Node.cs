using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Components.Rules;
using WorkFlow.Enums;
using WorkFlow.Exceptions;
using WorkFlow.Interfaces;
using WorkFlow.Interfaces.Entities;

namespace WorkFlow.Components
{
    public class Node
    {
        private Context Context { get; set; }

        #region Properties
        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public NodeType NodeType
        {
            get
            {
                return Enum.IsDefined(typeof(NodeType), Type) ? (NodeType)Type : NodeType.UNKNOWN;
            }
        }

        #region Rules
        private Rule4Approvers Rule4Approvers { get; set; }
        private Rule4Input Rule4Input { get; set; }
        private Rule4Output Rule4Output { get; set; }
        private Rule4Status Rule4Status { get; set; }
        #endregion
        #region Actions
        #endregion
        #endregion

        public bool CanInput
        {
            get
            {
                try
                {
                    if (NodeType == NodeType.Start) return true;
                    if (Parents.Any(n => n.Status == (int)NodeStatus.Error)) return false;
                    if (Parents.Count(n => n.Status == (int)NodeStatus.Disable) == Parents.Count) return false;
                    //if (SEQ < Parents.Max(n => n.SEQ)) return false;
                    return Rule4Input.CanInput(Parents.FindAll(n => n.Status != (int)NodeStatus.Disable).ToArray(), Context.Task.Variables);
                }
                catch
                {
                    return false;
                }
                
            }
        }
        public int Status
        {
            get
            {
                var status = (int)NodeStatus.Disable;
                if (Data != null)
                {
                    if (!Data.Status0.HasValue)
                    {
                        Data.Status0 = CalculateStatus();
                    }
                    status = Data.Status0.Value;
                }
                return status;
            }
        }
        //public int Status0
        //{
        //    get
        //    {
        //        var status = (int)NodeStatus.Disable;
        //        if (SEQ > 0 && SEQ >= Parents.Max(n => n.SEQ))
        //        {
        //            status = Data.Status;
        //        }
        //        return status;
        //    }
        //}
        public NodeStatus NodeStatus
        {
            get
            {
                return (NodeStatus)Status;
            }
        }
        //public NodeStatus NodeStatus0
        //{
        //    get
        //    {
        //        return (NodeStatus)Status0;
        //    }
        //}

        internal List<Node> Parents { get; private set; }
        public bool HasParents { get { return Parents != null && Parents.Count > 0; } }
        internal List<Node> Children { get; private set; }
        public bool HasChildren { get { return Children != null && Children.Count > 0; } }

        public int SEQ
        {
            get
            {
                var pmax = HasParents ? Parents.Max(pn => pn.SEQ) : 0;
                var imax = Context.Data != null && Context.Data.Nodes != null && Context.Data.Nodes.Length > 0 ? Context.Data.Nodes.Max(n => n.NodeID == this.Id ? n.SEQ : 0) : 0;

                return imax > 0 && imax >= pmax ? imax : 0;
            }
        }
        public int RoundNO
        {
            get
            {
                var data = this.Data;
                return data != null ? data.RoundNO : 0;
            }
        }

        internal IRNode Data
        {
            get
            {
                var seq = this.SEQ;
                return seq > 0 ? Context.Data.Nodes.First(n => n.NodeID.Equals(this.Id) && n.SEQ.Equals(seq)) : null;
            }
        }
        internal IRDetail[] Details
        {
            get
            {
                var data = this.Data;
                return data != null ? data.Details.Where(d => d.Enabled).ToArray() : null;
            }
        }

        

        public Node(IDNode dnode, Context context)
        {
            if (dnode == null) throw new ArgumentNullException("dnode", "Argument(dnode) is null!");
            if (context == null) throw new ArgumentNullException("context", "Argument(context) is null!");

            Context = context;
            Parents = new List<Node>();
            Children = new List<Node>();

            Initialize(dnode);
        }

        private void Initialize(IDNode dnode)
        {
            Id = dnode.ID;
            Code = dnode.Code;
            Name = dnode.Name;
            Type = dnode.Type;

            //Create Rules
            Rule4Approvers = dnode.RL4Approvers != null ? dnode.RL4Approvers.Flag == 99 ? new Rule4Approvers(dnode.RL4Approvers.Script) : new Rule4Approvers(dnode.RL4Approvers.Flag) : null;
            Rule4Input = dnode.RL4Input != null ? dnode.RL4Input.Flag == 99 ? new Rule4Input(dnode.RL4Input.Script) : new Rule4Input(dnode.RL4Input.Flag) : new Rule4Input(0);
            Rule4Output = dnode.RL4Output != null ? dnode.RL4Output.Flag == 99 ? new Rule4Output(dnode.RL4Output.Script) : new Rule4Output(dnode.RL4Output.Flag) : new Rule4Output(0);
            Rule4Status = dnode.RL4Status != null ? dnode.RL4Status.Flag == 99 ? new Rule4Status(dnode.RL4Status.Script) : new Rule4Status(dnode.RL4Status.Flag) : new Rule4Status(0);
        }
        private string[] GetApprovers(dynamic variables)
        {
            var approvers = new List<string>();
            string[] approvers1 = null;
            string[] approvers2 = null;

            approvers1 = (Rule4Approvers != null) ? Rule4Approvers.GetApprovers(variables) : null;
            //approvers2 = (Action4Approvers != null) ? Action4Approvers.GetApprovers(variables) : null;
            if (approvers1 != null) approvers.AddRange(approvers1);
            if (approvers2 != null) approvers.AddRange(approvers2);

            ReplaceApprovers(approvers);

            switch (this.NodeType)
            {
                case NodeType.Start:
                    if (approvers.Count <= 0)
                    {
                        if (string.IsNullOrWhiteSpace(Context.Data.OwnerID)) throw new SenderIsNullOrEmptyException(Context.Data.ID, Context.Data.BizCode);
                        approvers.Add(Context.Data.OwnerID.Trim());
                    }
                    break;
                case NodeType.Normal:
                    if (approvers.Count <= 0) throw new NoApproverFoundException(this.Id, this.Code, this.Name);
                    break;
                case NodeType.Control:
                    approvers.Clear();
                    break;
                case NodeType.End:
                    approvers.Clear();
                    break;
                default:
                    throw new UnsupportedNodeTypeException(this.Id, this.Code, this.Name, this.Type);
            }

            return approvers.Distinct().ToArray();
        }
        private Node[] GetOutputNodes(dynamic variables)
        {
            if (NodeType == NodeType.End) return null;

            Node[] nodes = null;
            switch (NodeStatus)
            {
                case NodeStatus.Approved:
                    nodes = Rule4Output.GetNodes(NodeStatus, Children.ToArray(), variables);
                    break;
                case NodeStatus.Rejected:
                    nodes = Rule4Output.GetNodes(NodeStatus, GetParentNodes(false), variables);
                    break;
                default:
                    return new Node[0];
            }
            if (nodes == null || nodes.Length <= 0) throw new NoOutputNodesFoundException(this);
            return nodes;
        }
        private void ReplaceApprovers(List<string> approvers)
        {
            if (approvers == null || !approvers.Any(a => WorkFlow.WFSENDER.Equals(a.Trim().ToUpper()))) return;
            if(!string.IsNullOrWhiteSpace(Context.Data.OwnerID) && !WorkFlow.WFSENDER.Equals(Context.Data.OwnerID.Trim().ToUpper()))
            {
                var index = -1;
                while((index = approvers.FindIndex(a => WorkFlow.WFSENDER.Equals(a.Trim().ToUpper()))) >= 0)
                {
                    approvers[index] = Context.Data.OwnerID.Trim();
                }
            }
        }
        private int CalculateStatus()
        {
            try
            {
                var status = (int)NodeStatus.None;
                if (CanInput)
                {
                    switch (this.NodeType)
                    {
                        case NodeType.Start:
                        case NodeType.Normal:
                            var details = Details;
                            status = (details == null || details.Length == 0) ? (int)NodeStatus.Disable : Rule4Status.GetStatus(details, Context.Task.Variables);
                            break;
                        case NodeType.Control:
                        case NodeType.End:
                            status = this.Data != null ? (int)NodeStatus.Approved : (int)NodeStatus.Disable;
                            break;
                    }
                    
                }
                else
                {
                    if (Parents.Any(n => n.Status == (int)NodeStatus.Error)) throw new ApplicationException("Parent nodes contain error status!");
                    status = (Parents.Count(n => n.Status == (int)NodeStatus.Disable) == Parents.Count) ? (int)NodeStatus.Disable : (int)NodeStatus.None;
                }
                return status;
            }
            catch (Exception err)
            {
                return (int)NodeStatus.Error;
            }
        }
        private DetailStatus ConvertDetailStatusFromAction(TaskAction action)
        {
            DetailStatus status = DetailStatus.None;
            switch (action)
            {
                case TaskAction.Send:
                case TaskAction.Approve:
                    status = DetailStatus.Approved;
                    break;
                case TaskAction.Reject:
                    status = DetailStatus.Rejected;
                    break;
            }
            return status;
        }
        

        internal void AppendTo(Node parent)
        {
            if (parent == null) throw new ArgumentNullException("parent", "Argument(parent) is null!");
            if (!Parents.Any(p => p.Id.Equals(parent.Id))) Parents.Add(parent);
        }

        internal void Append(Node child)
        {
            if (child == null) throw new ArgumentNullException("child", "Argument(child) is null!");
            child.AppendTo(this);
            if (!Children.Any(c => c.Id.Equals(child.Id))) Children.Add(child);
        }

        internal void Process(TaskAction action, IRTask task)
        {
            if (task == null) throw new ArgumentNullException("task", "Argument(task) is null!");
            if (!CanInput)
            {
                if(action != TaskAction.None) throw new NodeProcessException(this, "不能进入该节点！");
                return;
            }

            var user = task.CreatedBy;
            var status0 = this.Status;
            switch (action)
            {
                case TaskAction.None: //自动生成的Task
                    var approvers = GetApprovers(task.Variables);
                    if (approvers.Length == 0)
                    {
                        //缺省审批
                        Context.CreateNormalNodeData(user, Id, null, null);
                    }
                    else
                    {
                        Context.CreateNormalNodeData(user, Id, null, null, approvers);
                    }
                    
                    break;
                case TaskAction.Send: //首次发起
                    Context.CreateStartNodeData(user, Id, task.Comment, task.Parameter);
                    break;
                case TaskAction.Approve: //外部生成task, 通过
                case TaskAction.Reject: //外部生成task, 拒绝
                    switch (NodeStatus)
                    {
                        case NodeStatus.Processing:
                            Context.UpdateDetailData(user, Data.ID, task.DetailID.Value, ConvertDetailStatusFromAction(action), task.Comment, task.Parameter);
                            break;
                        case NodeStatus.None:
                            throw new NodeProcessException(this, "没到该节点审批！");
                        case NodeStatus.Disable:
                            throw new NodeProcessException(this, "该节点处于无效状态，暂时不能审批！");
                        case NodeStatus.Approved:
                        case NodeStatus.Rejected:
                            throw new NodeProcessException(this, "该节点已经完成审批！");
                        case NodeStatus.Error:
                            throw new NodeProcessException(this, "该节点状态目前有错误！");
                    }
                    break;

            }
            //更新节点状态
            var status = CalculateStatus();
            if (status != status0) ResetStatus();
            Context.UpdateNodeStatus(user, Data.ID, NodeStatus);

            //发邮件提醒用户当前操作处理情况


            //走到一下节点
            var nodes = GetOutputNodes(task.Variables);
            if (nodes != null && nodes.Length > 0)
            {
                foreach (var node in nodes)
                {
                    node.Process(TaskAction.None, Context.CreateTaskData(user, TaskAction.None));
                }
            }
        }

        internal void ResetStatus()
        {
            if (Data != null) Data.Status0 = null;
            if (HasChildren)
            {
                foreach(var child in Children)
                {
                    child.ResetStatus();
                }
            }
        }
        internal Node[] GetParentNodes(bool self, List<Node> all = null)
        {
            List<Node> nodes = all != null ? all : new List<Node>();

            if (nodes.Any(n => n.Id == this.Id)) return null;

            if (this.HasParents)
            {
                foreach (var node in this.Parents)
                {
                    node.GetParentNodes(true, nodes);
                }
            }

            if (self && NodeStatus != NodeStatus.Disable) nodes.Add(this);

            return nodes.ToArray();
        }
    }
}
