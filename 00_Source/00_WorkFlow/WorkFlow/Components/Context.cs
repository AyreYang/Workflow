using System;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Interfaces.Entities;

namespace WorkFlow.Components
{
    public abstract class Context
    {
        public IRTask Task { get; private set; }
        public IRWorkflow Data { get; private set; }
        public IRNode[] Nodes
        {
            get
            {
                var nodes = Data != null ? Data.Nodes : null;
                return nodes != null ? nodes : new IRNode[0];
            }
        }
        public Context()
        {
            ClearData();
        }

        internal IDWorkflow FetchWorkflow(Guid workflowId)
        {
            return FetchData<IDWorkflow>(workflowId);
        }

        internal void LoadData(TaskAction action, IRTask task)
        {
            IRWorkflow workflow = null;
            if (action == TaskAction.Send)
            {
                workflow = NewWorkflowData(task);
            }
            else
            {
                if (!task.InstID.HasValue) throw new ArgumentException("Argument(task.InstID) is null!", "task");
                workflow = FetchWorkflowData(task.InstID.Value);
            }

            if (workflow == null) throw new ApplicationException("Laoding WorkFlow Data Failed!");
            

            ClearData();
            Task = task;
            Data = workflow;
        }
        internal void ClearData()
        {
            Task = null;
            Data = null;
        }

        internal void UpdateNodeStatus(string user, Guid nodeId, NodeStatus status)
        {
            UpdateNodeStatus(user, nodeId, (int)status);
        }
        internal void CreateNormalNodeData(string user, Guid nodeId, string comment, string parameter, params string[] approvers)
        {
            CreateNodeData(user, nodeId, CalculateSEQ(), CalculateRoundNO(nodeId), comment, parameter, (int)DetailStatus.Processing, approvers);
        }
        internal void CreateStartNodeData(string user, Guid nodeId, string comment, string parameter)
        {
            CreateNodeData(user, nodeId, CalculateSEQ(), CalculateRoundNO(nodeId), comment, parameter, (int)DetailStatus.Approved, user);
        }
        internal void UpdateDetailData(string user, Guid nodeId, Guid detailId, DetailStatus status, string comment, string parameter)
        {
            UpdateDetailData(user, nodeId, detailId, (int)status, comment, parameter);
        }
        internal void UpdateWorkflowStatus(string user, WorkflowStatus status)
        {
            UpdateWorkflowStatus(user, (int)status);
        }
        internal IRTask CreateTaskData(string user, TaskAction action)
        {
            return CreateTaskData(user, Data.WorkflowID, Task.BizCode, (int)action, Task.Comment, Task.Parameter);
        }
        internal void SaveData(string user, Guid? taskId)
        {
            SaveWorkFlowData(user, taskId);
        }

        #region abstract methods
        protected abstract T FetchData<T>(Guid id);
        protected abstract IRWorkflow NewWorkflowData(IRTask task);
        protected abstract IRWorkflow FetchWorkflowData(Guid instId);
        protected abstract void UpdateWorkflowStatus(string user, int status);
        protected abstract void CreateNodeData(string user, Guid nodeId, int seq, int round, string comment, string parameter, int status, params string[] approvers);
        protected abstract void UpdateDetailData(string user, Guid nodeId, Guid detailId, int status, string comment, string parameter);
        protected abstract void UpdateNodeStatus(string user, Guid nodeId, int status);
        protected abstract IRTask CreateTaskData(string user, Guid workflowId, string bizCode, int action, string comment, string parameter);
        protected abstract void SaveWorkFlowData(string user, Guid? taskId);
        public abstract void Logging(Exception err);
        #endregion

        private int CalculateSEQ()
        {
            var max = Nodes.Length > 0 ? Nodes.Max(n => n.SEQ) : 0;
            return (int)++max;
        }
        private int CalculateRoundNO(Guid node)
        {
            var nodes = Nodes.Where(n => n.NodeID.Equals(node)).ToArray();
            var max = nodes.Length > 0 ? nodes.Max(n => n.RoundNO) : 0;
            return (int)++max;
        }
    }

    /*
    public abstract class Context1 : IDisposable
    {
        public Guid Id { get; private set; }
        public Guid WorkFlowId
        {
            get
            {
                return Entity != null ? Entity.Id : Guid.Empty;
            }
        }
        public string WorkFlowName
        {
            get
            {
                return Entity != null ? Entity.Name : null;
            }
        }

        internal IList<Node> Nodes { get; private set; }
        

        //public IEWorkFlow Entity { get; private set; }
        public IPWorkFlow Data { get; private set; }

        public Context1(IEWorkFlow workflow)
        {
            if (workflow == null) throw new ArgumentNullException("workflow");
            Entity = workflow;
            Nodes = new List<Node>();
        }

        #region abstract methods
        protected abstract IPWorkFlow FetchData(ITask task);
        public abstract void Dispose();
        public abstract void Logging(int type, string message, params object[] argms);

        protected abstract void UpdateNodeStatus(string user, Guid nid, int status);
        protected abstract void UpdateWorkflowStatus(string user, int status);
        protected abstract Guid CreateNodeData(string user, Guid node, int seq, int round, params string[] approvers);
        protected abstract void UpdateNodeData(string user, Guid node, int status, string comment, string parameter, params Guid[] bizId);
        protected abstract ITask CreateTask(string user, Guid workflow, int status);
        protected abstract void SaveWorkFlowData();
        #endregion

        internal void LoadData(ITask task)
        {
            var data = FetchData(task);
            if (data == null) throw new ApplicationException("Laoding WorkFlow Data Failed!");
            ClearData();
            Data = data;
        }
        internal void SaveData()
        {
            SaveWorkFlowData();
        }
        internal void UpdateNodeStatus(string user, Guid nid, NodeStatus status)
        {
            var node = Data.Nodes.FirstOrDefault(n => n.Id.Equals(nid));
            if (node == null) throw new ApplicationException(string.Format("Node({0}) doesn`t exist!", nid));
            if (node.Status != (int)status) UpdateNodeStatus(user, nid, (int)status);
        }
        internal void UpdateWorkflowStatus(string user, WorkflowStatus status)
        {
            if (Data.Status != (int)status) UpdateWorkflowStatus(user, (int)status);
        }
        internal Guid CreateNewNodeData(string user, Guid node, params string[] approvers)
        {
            return CreateNodeData(user, node, CalculateSEQ(), CalculateRoundNO(node), approvers);
        }
        internal Guid CreateExistedNodeData(string user, Guid node, int seq, params string[] approvers)
        {
            if (seq <= 0) throw new ApplicationException(string.Format("The SEQ({0}) of Node({1}) is illegal!", seq, node));
            return CreateNodeData(user, node, seq, CalculateRoundNO(node), approvers);
        }
        internal void ApproveNodeData(string user, Guid node, string comment, string parameter, Guid[] bizId)
        {
            UpdateNodeData(user, node, (int)NodeStatus.Approved, comment, parameter, bizId);
        }
        internal void RejectNodeData(string user, Guid node, string comment, string parameter, Guid[] bizId)
        {
            UpdateNodeData(user, node, (int)NodeStatus.Rejected, comment, parameter, bizId);
        }
        internal ITask CreateTask(string user, int status)
        {
            return CreateTask(user, WorkFlowId, status);
        }


        private int CalculateSEQ()
        {
            var max = Data != null && Data.Nodes != null && Data.Nodes.Count > 0 ? Data.Nodes.Max(n => n.Seq) : 0;
            return (int)++max;
        }
        private int CalculateRoundNO(Guid node)
        {
            var nodes = Data != null && Data.Nodes != null ? Data.Nodes.Where(n => n.NodeId.Equals(node)).ToArray() : null;
            var max = nodes != null && nodes.Length > 0 ? nodes.Max(n => n.RoundNO) : 0;
            return (int)++max;
        }

        internal void ClearData()
        {
            if (Data != null) Data.Dispose();

            Data = null;
        }

        internal void LoggingError(Exception err)
        {
            Logging(4, err.ToString());
        }

        
    }
    */
}
