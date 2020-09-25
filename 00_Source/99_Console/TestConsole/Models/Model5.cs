using Database.Interfaces;
using System;
using WorkFlow.Enums;
using WorkFlowEntities.Entities;

namespace TestConsole.Models
{
    public class Model5 : Model
    {
        public Model5(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("a064ace8-bd4f-47fd-84fb-f77045d3aafa"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";
            
            var ent_wf = new WF_DEF_Workflow();
            ent_wf.ID = Guid.NewGuid();
            ent_wf.Name = "工作流模型5";
            ent_wf.Code = "WorkFlowModel5";
            ent_wf.IsDeleted = false;
            ent_wf.CreatedBy = user;
            ent_wf.CreatedOn = DateTime.Now;
            ent_wf.LastModifiedBy = user;
            ent_wf.LastModifiedOn = DateTime.Now;

            var ent_nd0 = new WF_DEF_Node();
            ent_nd0.ID = Guid.NewGuid();
            ent_nd0.WorkflowID = ent_wf.ID;
            ent_nd0.Name = "发起节点";
            ent_nd0.Code = "SenderNode";
            ent_nd0.Type = 0;
            ent_nd0.IsDeleted = false;
            ent_nd0.CreatedBy = user;
            ent_nd0.CreatedOn = DateTime.Now;
            ent_nd0.LastModifiedBy = user;
            ent_nd0.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd0);

            #region Approval Node1-1

            var ent_nd11 = new WF_DEF_Node();
            ent_nd11.ID = Guid.NewGuid();
            ent_nd11.WorkflowID = ent_wf.ID;
            ent_nd11.Name = "审批节点1-1";
            ent_nd11.Code = "ApprovalNode1-1";
            ent_nd11.Type = 1;
            ent_nd11.IsDeleted = false;
            ent_nd11.CreatedBy = user;
            ent_nd11.CreatedOn = DateTime.Now;
            ent_nd11.LastModifiedBy = user;
            ent_nd11.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping11 = new WF_DEF_Mapping();
            ent_mapping11.ID = Guid.NewGuid();
            ent_mapping11.NodeID = ent_nd11.ID;
            ent_mapping11.ParentID = ent_nd0.ID;
            ent_mapping11.WorkflowID = ent_wf.ID;
            ent_mapping11.IsDeleted = false;
            ent_mapping11.CreatedBy = user;
            ent_mapping11.CreatedOn = DateTime.Now;
            ent_mapping11.LastModifiedBy = user;
            ent_mapping11.LastModifiedOn = DateTime.Now;
            ent_nd11.Parents.Add(ent_mapping11);

            //create rules
            var ent_rule11 = new WF_DEF_Rule();
            ent_rule11.ID = Guid.NewGuid();
            ent_rule11.NodeID = ent_nd11.ID;
            ent_rule11.Type = 0;
            ent_rule11.Flag = 99;
            ent_rule11.Name = "找人规则1-1";
            ent_rule11.Code = "FindApprovers1-1";
            ent_rule11.Script = @"return new string[]{ ""E0300084"" };";
            ent_rule11.IsDeleted = false;
            ent_rule11.CreatedBy = user;
            ent_rule11.CreatedOn = DateTime.Now;
            ent_rule11.LastModifiedBy = user;
            ent_rule11.LastModifiedOn = DateTime.Now;
            ent_nd11.RL_Approvers = ent_rule11.ID;
            ent_nd11.Rules.Add(ent_rule11);

            ent_wf.Nodes.Add(ent_nd11);
            #endregion

            #region Approval Node1-2

            var ent_nd12 = new WF_DEF_Node();
            ent_nd12.ID = Guid.NewGuid();
            ent_nd12.WorkflowID = ent_wf.ID;
            ent_nd12.Name = "审批节点1-2";
            ent_nd12.Code = "ApprovalNode1-2";
            ent_nd12.Type = 1;
            ent_nd12.IsDeleted = false;
            ent_nd12.CreatedBy = user;
            ent_nd12.CreatedOn = DateTime.Now;
            ent_nd12.LastModifiedBy = user;
            ent_nd12.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping12 = new WF_DEF_Mapping();
            ent_mapping12.ID = Guid.NewGuid();
            ent_mapping12.NodeID = ent_nd12.ID;
            ent_mapping12.ParentID = ent_nd11.ID;
            ent_mapping12.WorkflowID = ent_wf.ID;
            ent_mapping12.IsDeleted = false;
            ent_mapping12.CreatedBy = user;
            ent_mapping12.CreatedOn = DateTime.Now;
            ent_mapping12.LastModifiedBy = user;
            ent_mapping12.LastModifiedOn = DateTime.Now;
            ent_nd12.Parents.Add(ent_mapping12);

            //create rules
            var ent_rule12 = new WF_DEF_Rule();
            ent_rule12.ID = Guid.NewGuid();
            ent_rule12.NodeID = ent_nd12.ID;
            ent_rule12.Type = 0;
            ent_rule12.Flag = 99;
            ent_rule12.Name = "找人规则1-2";
            ent_rule12.Code = "FindApprovers1-2";
            ent_rule12.Script = @"return new string[]{ ""E0344652"" };";
            ent_rule12.IsDeleted = false;
            ent_rule12.CreatedBy = user;
            ent_rule12.CreatedOn = DateTime.Now;
            ent_rule12.LastModifiedBy = user;
            ent_rule12.LastModifiedOn = DateTime.Now;
            ent_nd12.RL_Approvers = ent_rule12.ID;
            ent_nd12.Rules.Add(ent_rule12);

            ent_wf.Nodes.Add(ent_nd12);
            #endregion

            #region Approval Node2-1

            var ent_nd21 = new WF_DEF_Node();
            ent_nd21.ID = Guid.NewGuid();
            ent_nd21.WorkflowID = ent_wf.ID;
            ent_nd21.Name = "审批节点2-1";
            ent_nd21.Code = "ApprovalNode2-1";
            ent_nd21.Type = 1;
            ent_nd21.IsDeleted = false;
            ent_nd21.CreatedBy = user;
            ent_nd21.CreatedOn = DateTime.Now;
            ent_nd21.LastModifiedBy = user;
            ent_nd21.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping21 = new WF_DEF_Mapping();
            ent_mapping21.ID = Guid.NewGuid();
            ent_mapping21.NodeID = ent_nd21.ID;
            ent_mapping21.ParentID = ent_nd0.ID;
            ent_mapping21.WorkflowID = ent_wf.ID;
            ent_mapping21.IsDeleted = false;
            ent_mapping21.CreatedBy = user;
            ent_mapping21.CreatedOn = DateTime.Now;
            ent_mapping21.LastModifiedBy = user;
            ent_mapping21.LastModifiedOn = DateTime.Now;
            ent_nd21.Parents.Add(ent_mapping21);

            //create rules
            var ent_rule21 = new WF_DEF_Rule();
            ent_rule21.ID = Guid.NewGuid();
            ent_rule21.NodeID = ent_nd21.ID;
            ent_rule21.Type = 0;
            ent_rule21.Flag = 99;
            ent_rule21.Name = "找人规则2-1";
            ent_rule21.Code = "FindApprovers2-1";
            ent_rule21.Script = @"return new string[]{ ""E0299598"" };";
            ent_rule21.IsDeleted = false;
            ent_rule21.CreatedBy = user;
            ent_rule21.CreatedOn = DateTime.Now;
            ent_rule21.LastModifiedBy = user;
            ent_rule21.LastModifiedOn = DateTime.Now;
            ent_nd21.RL_Approvers = ent_rule21.ID;
            ent_nd21.Rules.Add(ent_rule21);

            ent_wf.Nodes.Add(ent_nd21);
            #endregion

            #region Approval Node2-2

            var ent_nd22 = new WF_DEF_Node();
            ent_nd22.ID = Guid.NewGuid();
            ent_nd22.WorkflowID = ent_wf.ID;
            ent_nd22.Name = "审批节点2-2";
            ent_nd22.Code = "ApprovalNode2-2";
            ent_nd22.Type = 1;
            ent_nd22.IsDeleted = false;
            ent_nd22.CreatedBy = user;
            ent_nd22.CreatedOn = DateTime.Now;
            ent_nd22.LastModifiedBy = user;
            ent_nd22.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping22 = new WF_DEF_Mapping();
            ent_mapping22.ID = Guid.NewGuid();
            ent_mapping22.NodeID = ent_nd22.ID;
            ent_mapping22.ParentID = ent_nd21.ID;
            ent_mapping22.WorkflowID = ent_wf.ID;
            ent_mapping22.IsDeleted = false;
            ent_mapping22.CreatedBy = user;
            ent_mapping22.CreatedOn = DateTime.Now;
            ent_mapping22.LastModifiedBy = user;
            ent_mapping22.LastModifiedOn = DateTime.Now;
            ent_nd22.Parents.Add(ent_mapping22);

            //create rules
            var ent_rule22 = new WF_DEF_Rule();
            ent_rule22.ID = Guid.NewGuid();
            ent_rule22.NodeID = ent_nd22.ID;
            ent_rule22.Type = 0;
            ent_rule22.Flag = 99;
            ent_rule22.Name = "找人规则2-2";
            ent_rule22.Code = "FindApprovers2-2";
            ent_rule22.Script = @"return new string[]{ ""E0123456"" };";
            ent_rule22.IsDeleted = false;
            ent_rule22.CreatedBy = user;
            ent_rule22.CreatedOn = DateTime.Now;
            ent_rule22.LastModifiedBy = user;
            ent_rule22.LastModifiedOn = DateTime.Now;
            ent_nd22.RL_Approvers = ent_rule22.ID;
            ent_nd22.Rules.Add(ent_rule22);

            ent_wf.Nodes.Add(ent_nd22);
            #endregion

            //create mapping
            var ent_mapping3 = new WF_DEF_Mapping();
            ent_mapping3.ID = Guid.NewGuid();
            ent_mapping3.NodeID = ent_nd12.ID;
            ent_mapping3.ParentID = ent_nd21.ID;
            ent_mapping3.WorkflowID = ent_wf.ID;
            ent_mapping3.IsDeleted = false;
            ent_mapping3.CreatedBy = user;
            ent_mapping3.CreatedOn = DateTime.Now;
            ent_mapping3.LastModifiedBy = user;
            ent_mapping3.LastModifiedOn = DateTime.Now;
            ent_nd12.Parents.Add(ent_mapping3);

            var ent_nd9 = new WF_DEF_Node();
            ent_nd9.ID = Guid.NewGuid();
            ent_nd9.WorkflowID = ent_wf.ID;
            ent_nd9.Name = "结束节点";
            ent_nd9.Code = "EndNode";
            ent_nd9.Type = 9;
            ent_nd9.IsDeleted = false;
            ent_nd9.CreatedBy = user;
            ent_nd9.CreatedOn = DateTime.Now;
            ent_nd9.LastModifiedBy = user;
            ent_nd9.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd9);

            ent_wf.Save(Accessor);
            
        }

        public void AddRules(string user)
        {
            var ent_wf = this.GetWorkflow();
            var ent_nd12 = ent_wf.Nodes.FirstOrDefault(n => n.Code.Equals("ApprovalNode1-2"));
            var ent_nd22 = ent_wf.Nodes.FirstOrDefault(n => n.Code.Equals("ApprovalNode2-2"));

            //create rules
            var ent_rule12 = new WF_DEF_Rule();
            ent_rule12.ID = Guid.NewGuid();
            ent_rule12.NodeID = ent_nd12.ID;
            ent_rule12.Type = (int)RuleType.Rule4Output;
            ent_rule12.Flag = 99;
            ent_rule12.Name = "输出规则1-2";
            ent_rule12.Code = "Output1-2";
            ent_rule12.Script = @"
            INode[] inodes = null;
            switch ((NodeStatus)node.Status)
            {
                case NodeStatus.Approved:
                    inodes = nodes;
                    break;
                case NodeStatus.Rejected:
                    inodes = nodes.Where(n => n.Code == ""ApprovalNode1-1"").ToArray();
                    break;
                default:
                    inodes = new INode[0];
                    break;

            }
            return inodes;
";
            ent_rule12.IsDeleted = false;
            ent_rule12.CreatedBy = user;
            ent_rule12.CreatedOn = DateTime.Now;
            ent_rule12.LastModifiedBy = user;
            ent_rule12.LastModifiedOn = DateTime.Now;
            ent_nd12.RL_Output = ent_rule12.ID;
            ent_nd12.Rules.Add(ent_rule12);


            var ent_rule22 = new WF_DEF_Rule();
            ent_rule22.ID = Guid.NewGuid();
            ent_rule22.NodeID = ent_nd22.ID;
            ent_rule22.Type = (int)RuleType.Rule4Output;
            ent_rule22.Flag = 99;
            ent_rule22.Name = "输出规则2-2";
            ent_rule22.Code = "Output2-2";
            ent_rule22.Script = @"
            INode[] inodes = null;
            switch ((NodeStatus)node.Status)
            {
                case NodeStatus.Approved:
                    inodes = nodes;
                    break;
                case NodeStatus.Rejected:
                    inodes = nodes.Where(n => n.Code == ""ApprovalNode2-1"").ToArray();
                    break;
                default:
                    inodes = new INode[0];
                    break;

            }
            return inodes;
";
            ent_rule22.IsDeleted = false;
            ent_rule22.CreatedBy = user;
            ent_rule22.CreatedOn = DateTime.Now;
            ent_rule22.LastModifiedBy = user;
            ent_rule22.LastModifiedOn = DateTime.Now;
            ent_nd22.RL_Output = ent_rule22.ID;
            ent_nd22.Rules.Add(ent_rule22);

            ent_wf.Save(this.Accessor);
        }
    }
}
