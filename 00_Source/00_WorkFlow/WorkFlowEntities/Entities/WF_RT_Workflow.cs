using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_RT_Workflows")]
    public class WF_RT_Workflow : TableEntity, IRWorkflow
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid WorkflowID { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 30, false, false)]
        public string BizCode { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int Status { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50, false, false)]
        public string OwnerID { get; set; }

        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int ProcStatus { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? TaskID { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }

        [DBForeignAttribute("ID=>InstID")]
        public DBRefList<WF_RT_Node> Nodes { get; set; }
        IRNode[] IRWorkflow.Nodes => Nodes.Entities;
    }
}
