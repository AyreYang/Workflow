using Database.Interfaces;
using System;
using WorkFlowEntities.Entities;

namespace TestConsole.Models
{
    public class Model2 : Model
    {
        public Model2(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("64443679-5080-424d-baf7-397f3fe4f21c"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";
            
            var ent_wf = new WF_DEF_Workflow();
            ent_wf.ID = Guid.NewGuid();
            ent_wf.Name = "工作流模型2";
            ent_wf.Code = "WorkFlowModel2";
            ent_wf.IsDeleted = false;
            ent_wf.CreatedBy = user;
            ent_wf.CreatedOn = DateTime.Now;
            ent_wf.LastModifiedBy = user;
            ent_wf.LastModifiedOn = DateTime.Now;

            var ent_nd1 = new WF_DEF_Node();
            ent_nd1.ID = Guid.NewGuid();
            ent_nd1.WorkflowID = ent_wf.ID;
            ent_nd1.Name = "发起节点";
            ent_nd1.Code = "SenderNode";
            ent_nd1.Type = 0;
            ent_nd1.IsDeleted = false;
            ent_nd1.CreatedBy = user;
            ent_nd1.CreatedOn = DateTime.Now;
            ent_nd1.LastModifiedBy = user;
            ent_nd1.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd1);

            #region Approval Node

            var ent_nd2 = new WF_DEF_Node();
            ent_nd2.ID = Guid.NewGuid();
            ent_nd2.WorkflowID = ent_wf.ID;
            ent_nd2.Name = "审批节点";
            ent_nd2.Code = "ApprovalNode";
            ent_nd2.Type = 1;
            ent_nd2.IsDeleted = false;
            ent_nd2.CreatedBy = user;
            ent_nd2.CreatedOn = DateTime.Now;
            ent_nd2.LastModifiedBy = user;
            ent_nd2.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping = new WF_DEF_Mapping();
            ent_mapping.ID = Guid.NewGuid();
            ent_mapping.NodeID = ent_nd2.ID;
            ent_mapping.WorkflowID = ent_wf.ID;
            ent_mapping.ParentID = ent_nd1.ID;
            ent_mapping.IsDeleted = false;
            ent_mapping.CreatedBy = user;
            ent_mapping.CreatedOn = DateTime.Now;
            ent_mapping.LastModifiedBy = user;
            ent_mapping.LastModifiedOn = DateTime.Now;
            ent_nd2.Parents.Add(ent_mapping);

            //create rules
            var ent_rule1 = new WF_DEF_Rule();
            ent_rule1.ID = Guid.NewGuid();
            ent_rule1.NodeID = ent_nd2.ID;
            ent_rule1.Type = 0;
            ent_rule1.Flag = 99;
            ent_rule1.Name = "找人规则";
            ent_rule1.Code = "FindApprovers";
            ent_rule1.Script = @"return new string[]{ ""E0300084"", ""E0344652"" };";
            ent_rule1.IsDeleted = false;
            ent_rule1.CreatedBy = user;
            ent_rule1.CreatedOn = DateTime.Now;
            ent_rule1.LastModifiedBy = user;
            ent_rule1.LastModifiedOn = DateTime.Now;
            ent_nd2.RL_Approvers = ent_rule1.ID;
            ent_nd2.Rules.Add(ent_rule1);

            ent_wf.Nodes.Add(ent_nd2);
            #endregion


            var ent_nd3 = new WF_DEF_Node();
            ent_nd3.ID = Guid.NewGuid();
            ent_nd3.WorkflowID = ent_wf.ID;
            ent_nd3.Name = "结束节点";
            ent_nd3.Code = "EndNode";
            ent_nd3.Type = 9;
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
