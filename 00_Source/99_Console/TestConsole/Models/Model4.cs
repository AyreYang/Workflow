using Database.Interfaces;
using System;
using WorkFlowEngine.Entities;

namespace TestConsole.Models
{
    public class Model4 : Model
    {
        public Model4(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("e2a282f4-1ee0-4873-a27f-249fe904fe34"))
        {
        }
        public override void CreateWorkFlow()
        {
            var user = "E0350802";

            var ent_wf = new WF_ENT_WorkFlow();
            ent_wf.Id = Guid.NewGuid();
            ent_wf.NameCN = "工作流模型4";
            ent_wf.NameEN = "WorkFlowModel4";
            ent_wf.Version = 1;
            ent_wf.IsDeleted = false;
            ent_wf.CreatedBy = user;
            ent_wf.CreatedOn = DateTime.Now;
            ent_wf.LastModifiedBy = user;
            ent_wf.LastModifiedOn = DateTime.Now;

            var ent_nd1 = new WF_ENT_Node();
            ent_nd1.ID = Guid.NewGuid().ToString();
            ent_nd1.WorkFlowID = ent_wf.ID;
            ent_nd1.NameCN = "发起节点";
            ent_nd1.NameEN = "SenderNode";
            ent_nd1.Type = 0;
            ent_nd1.IsDeleted = false;
            ent_nd1.CreatedBy = user;
            ent_nd1.CreatedOn = DateTime.Now;
            ent_nd1.LastModifiedBy = user;
            ent_nd1.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd1);

            #region Approval Node2

            var ent_nd2 = new WF_ENT_Node();
            ent_nd2.ID = Guid.NewGuid().ToString();
            ent_nd2.WorkFlowID = ent_wf.ID;
            ent_nd2.NameCN = "审批节点1";
            ent_nd2.NameEN = "ApprovalNode1";
            ent_nd2.Type = 1;
            ent_nd2.IsDeleted = false;
            ent_nd2.CreatedBy = user;
            ent_nd2.CreatedOn = DateTime.Now;
            ent_nd2.LastModifiedBy = user;
            ent_nd2.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping1 = new WF_ENT_NodeMapping();
            ent_mapping1.ID = Guid.NewGuid().ToString();
            ent_mapping1.NodeID = ent_nd2.ID;
            ent_mapping1.ParentID = ent_nd1.ID;
            ent_mapping1.IsDeleted = false;
            ent_mapping1.CreatedBy = user;
            ent_mapping1.CreatedOn = DateTime.Now;
            ent_mapping1.LastModifiedBy = user;
            ent_mapping1.LastModifiedOn = DateTime.Now;
            ent_nd2.Mappings.Add(ent_mapping1);

            //create rules
            var ent_rule1 = new WF_ENT_NodeRule();
            ent_rule1.ID = Guid.NewGuid().ToString();
            ent_rule1.NodeID = ent_nd2.ID;
            ent_rule1.RuleType = 0;
            ent_rule1.ProcType = 0;
            ent_rule1.NameCN = "找人规则1";
            ent_rule1.NameEN = "FindApprovers1";
            ent_rule1.Content = "E0300084;E0344652;";
            ent_rule1.IsDeleted = false;
            ent_rule1.CreatedBy = user;
            ent_rule1.CreatedOn = DateTime.Now;
            ent_rule1.LastModifiedBy = user;
            ent_rule1.LastModifiedOn = DateTime.Now;
            ent_nd2.Rules.Add(ent_rule1);

            ent_wf.Nodes.Add(ent_nd2);
            #endregion

            #region Approval Node3

            var ent_nd3 = new WF_ENT_Node();
            ent_nd3.ID = Guid.NewGuid().ToString();
            ent_nd3.WorkFlowID = ent_wf.ID;
            ent_nd3.NameCN = "审批节点2";
            ent_nd3.NameEN = "ApprovalNode2";
            ent_nd3.Type = 1;
            ent_nd3.IsDeleted = false;
            ent_nd3.CreatedBy = user;
            ent_nd3.CreatedOn = DateTime.Now;
            ent_nd3.LastModifiedBy = user;
            ent_nd3.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping2 = new WF_ENT_NodeMapping();
            ent_mapping2.ID = Guid.NewGuid().ToString();
            ent_mapping2.NodeID = ent_nd3.ID;
            ent_mapping2.ParentID = ent_nd2.ID;
            ent_mapping2.IsDeleted = false;
            ent_mapping2.CreatedBy = user;
            ent_mapping2.CreatedOn = DateTime.Now;
            ent_mapping2.LastModifiedBy = user;
            ent_mapping2.LastModifiedOn = DateTime.Now;
            ent_nd3.Mappings.Add(ent_mapping2);

            //create rules
            var ent_rule2 = new WF_ENT_NodeRule();
            ent_rule2.ID = Guid.NewGuid().ToString();
            ent_rule2.NodeID = ent_nd3.ID;
            ent_rule2.RuleType = 0;
            ent_rule2.ProcType = 0;
            ent_rule2.NameCN = "找人规则2";
            ent_rule2.NameEN = "FindApprovers2";
            ent_rule2.Content = "E0299598;E01234567";
            ent_rule2.IsDeleted = false;
            ent_rule2.CreatedBy = user;
            ent_rule2.CreatedOn = DateTime.Now;
            ent_rule2.LastModifiedBy = user;
            ent_rule2.LastModifiedOn = DateTime.Now;
            ent_nd3.Rules.Add(ent_rule2);

            ent_wf.Nodes.Add(ent_nd3);
            #endregion


            var ent_nd4 = new WF_ENT_Node();
            ent_nd4.ID = Guid.NewGuid().ToString();
            ent_nd4.WorkFlowID = ent_wf.ID;
            ent_nd4.NameCN = "结束节点";
            ent_nd4.NameEN = "EndNode";
            ent_nd4.Type = 2;
            ent_nd4.IsDeleted = false;
            ent_nd4.CreatedBy = user;
            ent_nd4.CreatedOn = DateTime.Now;
            ent_nd4.LastModifiedBy = user;
            ent_nd4.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd4);

            ent_wf.Save(Accessor);
        }
    }
}
