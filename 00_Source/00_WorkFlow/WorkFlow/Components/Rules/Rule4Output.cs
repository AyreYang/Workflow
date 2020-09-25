using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Exceptions;

namespace WorkFlow.Components.Rules
{
    public class Rule4Output : Rule
    {
        private const string Template = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public class Express
            {
                public object[] Test(string status, dynamic[] nodes, dynamic parameter)
                {
                    {body}
                }
            }";

        public Rule4Output(string script) : base(script)
        {
            this.SetFlag(99);
        }

        public Rule4Output(int flag) : base()
        {
            this.SetFlag(flag);
            switch (flag)
            {
                case 0:
                    this.Initialize(this, this.GetType().GetMethod("Rule0"));
                    break;
                default:
                    throw new ArgumentException(string.Format("flag({0}) is unsupported!", flag), "flag");
            }
        }

        protected override string BuildSource(string script, out string className, out string methodName)
        {
            className = "Express";
            methodName = "Test";
            return Template.Replace("{body}", script);
        }

        public Node[] GetNodes(NodeStatus status, Node[] nodes, dynamic parameter)
        {
            var list = new List<Node>();
            var result = this.Test(new object[] { status.ToString(), nodes, parameter });
            if (result != null)
            {
                var objects = result as object[];
                if (objects != null)
                {
                    foreach (dynamic obj in objects)
                    {
                        if (!(obj is Node)) throw new RuleReturnValueInvalidException<Node>(obj);
                        var node0 = obj as Node;
                        var node = nodes.FirstOrDefault(n => n.Id == node0.Id);
                        if (node == null) throw new NoOutputNodesFoundException(node0);
                        if (node != null && !list.Any(n => n.Id == node.Id))
                        {
                            list.Add(node);
                        }
                    }
                }
                else
                {
                    throw new RuleReturnValueInvalidException<Node[]>(result);
                }
            }
            return list.ToArray();
        }

        public Node[] Rule0(string status, Node[] nodes, dynamic parameter)
        {
            Node[] inodes = null;
            switch (Enum.Parse(typeof(NodeStatus), status))
            {
                case NodeStatus.Approved:
                    inodes = nodes;
                    break;
                case NodeStatus.Rejected:
                    inodes = nodes.Where(n => n.Type == (int)NodeType.Start).ToArray();
                    break;
                default:
                    inodes = new Node[0];
                    break;

            }
            return inodes;
        }
    }
}
