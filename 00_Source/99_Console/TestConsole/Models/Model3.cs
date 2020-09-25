using Database.Interfaces;
using System;
using WorkFlowEntities.Entities;

namespace TestConsole.Models
{
    public class Model3 : Model
    {
        public Model3(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("aa4e39f2-f63d-4036-be38-4755194916a6"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";
            
            var ent_wf = new WF_DEF_Workflow();
            ent_wf.ID = Guid.NewGuid();
            ent_wf.Name = "工作流模型3";
            ent_wf.Code = "WorkFlowModel3";
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

            #region Approval Node1

            var ent_nd2 = new WF_DEF_Node();
            ent_nd2.ID = Guid.NewGuid();
            ent_nd2.WorkflowID = ent_wf.ID;
            ent_nd2.Name = "审批节点1";
            ent_nd2.Code = "ApprovalNode1";
            ent_nd2.Type = 1;
            ent_nd2.IsDeleted = false;
            ent_nd2.CreatedBy = user;
            ent_nd2.CreatedOn = DateTime.Now;
            ent_nd2.LastModifiedBy = user;
            ent_nd2.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping1 = new WF_DEF_Mapping();
            ent_mapping1.ID = Guid.NewGuid();
            ent_mapping1.NodeID = ent_nd2.ID;
            ent_mapping1.ParentID = ent_nd1.ID;
            ent_mapping1.WorkflowID = ent_wf.ID;
            ent_mapping1.IsDeleted = false;
            ent_mapping1.CreatedBy = user;
            ent_mapping1.CreatedOn = DateTime.Now;
            ent_mapping1.LastModifiedBy = user;
            ent_mapping1.LastModifiedOn = DateTime.Now;
            ent_nd2.Parents.Add(ent_mapping1);

            //create rules
            var ent_rule1 = new WF_DEF_Rule();
            ent_rule1.ID = Guid.NewGuid();
            ent_rule1.NodeID = ent_nd2.ID;
            ent_rule1.Type = 0;
            ent_rule1.Flag = 99;
            ent_rule1.Name = "找人规则1";
            ent_rule1.Code = "FindApprovers1";
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

            #region Approval Node2

            var ent_nd3 = new WF_DEF_Node();
            ent_nd3.ID = Guid.NewGuid();
            ent_nd3.WorkflowID = ent_wf.ID;
            ent_nd3.Name = "审批节点2";
            ent_nd3.Code = "ApprovalNode2";
            ent_nd3.Type = 1;
            ent_nd3.IsDeleted = false;
            ent_nd3.CreatedBy = user;
            ent_nd3.CreatedOn = DateTime.Now;
            ent_nd3.LastModifiedBy = user;
            ent_nd3.LastModifiedOn = DateTime.Now;

            //create mapping
            var ent_mapping2 = new WF_DEF_Mapping();
            ent_mapping2.ID = Guid.NewGuid();
            ent_mapping2.NodeID = ent_nd3.ID;
            ent_mapping2.ParentID = ent_nd1.ID;
            ent_mapping2.WorkflowID = ent_wf.ID;
            ent_mapping2.IsDeleted = false;
            ent_mapping2.CreatedBy = user;
            ent_mapping2.CreatedOn = DateTime.Now;
            ent_mapping2.LastModifiedBy = user;
            ent_mapping2.LastModifiedOn = DateTime.Now;
            ent_nd3.Parents.Add(ent_mapping2);

            //create rules
            var ent_rule2 = new WF_DEF_Rule();
            ent_rule2.ID = Guid.NewGuid();
            ent_rule2.NodeID = ent_nd3.ID;
            ent_rule2.Type = 0;
            ent_rule2.Flag = 99;
            ent_rule2.Name = "找人规则2";
            ent_rule2.Code = "FindApprovers2";
            ent_rule2.Script = @"return new string[]{ ""E0299598"", ""E01234567"" };";
            ent_rule2.IsDeleted = false;
            ent_rule2.CreatedBy = user;
            ent_rule2.CreatedOn = DateTime.Now;
            ent_rule2.LastModifiedBy = user;
            ent_rule2.LastModifiedOn = DateTime.Now;
            ent_nd3.RL_Approvers = ent_rule2.ID;
            ent_nd3.Rules.Add(ent_rule2);

            ent_wf.Nodes.Add(ent_nd3);
            #endregion


            var ent_nd4 = new WF_DEF_Node();
            ent_nd4.ID = Guid.NewGuid();
            ent_nd4.WorkflowID = ent_wf.ID;
            ent_nd4.Name = "结束节点";
            ent_nd4.Code = "EndNode";
            ent_nd4.Type = 9;
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
