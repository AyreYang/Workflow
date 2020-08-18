using Database.Interfaces;
using System;
using WorkFlowEngine.Entities;

namespace TestConsole.Models
{
    public class Model2 : Model
    {
        public Model2(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("57616ed4-9d55-4f4b-a960-2b47a855686c"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";

            var ent_wf = new WF_ENT_WorkFlow();
            ent_wf.Id = Guid.NewGuid();
            ent_wf.NameCN = "工作流模型2";
            ent_wf.NameEN = "WorkFlowModel2";
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

            #region Approval Node

            var ent_nd2 = new WF_ENT_Node();
            ent_nd2.ID = Guid.NewGuid().ToString();
            ent_nd2.WorkFlowID = ent_wf.ID;
            ent_nd2.NameCN = "审批节点";
            ent_nd2.NameEN = "ApprovalNode";
            ent_nd2.Type = 1;
            ent_nd2.IsDeleted = false;
            ent_nd2.CreatedBy = user;
            ent_nd2.CreatedOn = DateTime.Now;
            ent_nd2.LastModifiedBy = user;
            ent_nd2.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping = new WF_ENT_NodeMapping();
            ent_mapping.ID = Guid.NewGuid().ToString();
            ent_mapping.NodeID = ent_nd2.ID;
            ent_mapping.ParentID = ent_nd1.ID;
            ent_mapping.IsDeleted = false;
            ent_mapping.CreatedBy = user;
            ent_mapping.CreatedOn = DateTime.Now;
            ent_mapping.LastModifiedBy = user;
            ent_mapping.LastModifiedOn = DateTime.Now;
            ent_nd2.Mappings.Add(ent_mapping);

            //create rules
            var ent_rule1 = new WF_ENT_NodeRule();
            ent_rule1.ID = Guid.NewGuid().ToString();
            ent_rule1.NodeID = ent_nd2.ID;
            ent_rule1.RuleType = 0;
            ent_rule1.ProcType = 0;
            ent_rule1.NameCN = "找人规则";
            ent_rule1.NameEN = "FindApprovers";
            ent_rule1.Content = "E0300084;E0344652;";
            ent_rule1.IsDeleted = false;
            ent_rule1.CreatedBy = user;
            ent_rule1.CreatedOn = DateTime.Now;
            ent_rule1.LastModifiedBy = user;
            ent_rule1.LastModifiedOn = DateTime.Now;
            ent_nd2.Rules.Add(ent_rule1);

            ent_wf.Nodes.Add(ent_nd2);
            #endregion


            var ent_nd3 = new WF_ENT_Node();
            ent_nd3.ID = Guid.NewGuid().ToString();
            ent_nd3.WorkFlowID = ent_wf.ID;
            ent_nd3.NameCN = "结束节点";
            ent_nd3.NameEN = "EndNode";
            ent_nd3.Type = 2;
            ent_nd3.IsDeleted = false;
            ent_nd3.CreatedBy = user;
            ent_nd3.CreatedOn = DateTime.Now;
            ent_nd3.LastModifiedBy = user;
            ent_nd3.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd3);

            ent_wf.Save(Accessor);
        }
    }
}
