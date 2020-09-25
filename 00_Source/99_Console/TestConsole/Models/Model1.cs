using Database.Interfaces;
using System;
using WorkFlowEntities.Entities;

namespace TestConsole.Models
{
    public class Model1 : Model
    {
        public Model1(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("5d88f343-f2e7-45b0-b0a0-c9cb4d751434"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";

            var ent_wf = new WF_DEF_Workflow();
            ent_wf.ID = Guid.NewGuid();
            ent_wf.Code = "WorkFlowModel1";
            ent_wf.Name = "工作流模型1";
            ent_wf.Description = null;
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

            var ent_nd2 = new WF_DEF_Node();
            ent_nd2.ID = Guid.NewGuid();
            ent_nd2.WorkflowID = ent_wf.ID;
            ent_nd2.Name = "结束节点";
            ent_nd2.Code = "EndNode";
            ent_nd2.Type = 9;
            ent_nd2.IsDeleted = false;
            ent_nd2.CreatedBy = user;
            ent_nd2.CreatedOn = DateTime.Now;
            ent_nd2.LastModifiedBy = user;
            ent_nd2.LastModifiedOn = DateTime.Now;
            ent_wf.Nodes.Add(ent_nd2);

            ent_wf.Save(Accessor);
        }
    }
}
