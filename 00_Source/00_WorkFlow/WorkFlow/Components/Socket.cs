using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Interfaces;

namespace WorkFlow.Components
{
    public abstract class Socket
    {
        protected Node Host { get; private set; }
        protected Context Context { get; private set; }
        protected IList<Node> Nodes { get; set; }

        

        public Socket(Node host, Context context, IList<Node> nodes)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (context == null) throw new ArgumentNullException("context");
            if (nodes == null) throw new ArgumentNullException("nodes");

            Host = host;
            Context = context;
            Nodes = nodes;
        }
    }

    public class Input : Socket
    {
        public bool CanInput
        {
            get
            {
                return Host.Type == NodeType.Sender ? true : Host.Entity.RULE4Input(Nodes.ToArray());
            }
        }
        public Input(Node host, Context context, IList<Node> nodes) : base(host, context, nodes) { }
    }

    public class Output : Socket
    {
        public Output(Node host, Context context, IList<Node> nodes) : base(host, context, nodes) { }

        private OPResult TestRule()
        {
            OPResult result = null;
            if (Host.HasDetails)
            {
                var nodes = new List<INode>();
                nodes.Add(Host);
                nodes.AddRange(Nodes);
                result = Host.Entity.RULE4Output(Host.Details.ToArray(), nodes.ToArray());
            }
            return result;
        }

        public NodeStatus Status
        {
            get
            {
                var status = NodeStatus.None;
                if (Host.Type == NodeType.End)
                {
                    status = NodeStatus.Approved;
                }
                else
                {
                    if (Host.HasDetails)
                    {
                        var opresult = TestRule();
                        if (opresult != null)
                        {
                            switch (opresult.Command)
                            {
                                case OPCommand.none:
                                    status = NodeStatus.Processing;
                                    break;
                                case OPCommand.approve:
                                    status = NodeStatus.Approved;
                                    break;
                                case OPCommand.reject:
                                    status = NodeStatus.Rejected;
                                    break;
                            }
                        }
                        else
                        {
                            status = NodeStatus.Error;
                        }
                    }
                }
                
                return status;
            }
        }

        public OPResult Go()
        {
            return TestRule();
        }
    }

    public class OPResult
    {
        public OPCommand Command { get; set; }
        private List<INode> nodes { get; set; }
        internal IList<Node> Nodes 
        { 
            get 
            { 
                return nodes.Select(n => n as Node).ToArray(); 
            }
        }
        public bool HasNodes { get { return nodes.Count > 0; } }

        public OPResult()
        {
            Command = OPCommand.none;
            nodes = new List<INode>();
        }

        public void AddNodes(params INode[] nodes)
        {
            if (nodes == null || nodes.Length <= 0) return;

            foreach(var node in nodes)
            {
                if (!(node is Node)) throw new ApplicationException(string.Format("Node({0}) is valid!", node.Id));
                if (!this.nodes.Contains(node)) this.nodes.Add(node);
            }
        }
    }
}
