using Database.Interfaces;
using System;
using WorkFlowEngine.Entities;

namespace TestConsole.Models
{
    public class Model1 : Model
    {
        public Model1(IDatabaseAccessor accessor) : base(accessor, Guid.Parse("ba9a9dc8-2101-4ae9-8ad3-bd826ac66393"))
        {
        }

        public override void CreateWorkFlow()
        {
            var user = "E0350802";

            var ent_wf = new WF_ENT_WorkFlow();
            ent_wf.Id = Guid.NewGuid();
            ent_wf.NameCN = "工作流模型1";
            ent_wf.NameEN = "WorkFlowModel1";
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

            var ent_nd2 = new WF_ENT_Node();
            ent_nd2.ID = Guid.NewGuid().ToString();
            ent_nd2.WorkFlowID = ent_wf.ID;
            ent_nd2.NameCN = "结束节点";
            ent_nd2.NameEN = "EndNode";
            ent_nd2.Type = 2;
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
