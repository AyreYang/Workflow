using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_RT_Nodes")]
    public class WF_RT_Node : TableEntity, IRNode
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid NodeID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid InstID { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int SEQ { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int RoundNO { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int Status { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }

        [DBForeignAttribute("ID=>NodeID")]
        public DBRefList<WF_RT_Detail> Details { get; set; }
        IRDetail[] IRNode.Details => Details.Entities;

        public int? Status0 { get; set; }
    }
}
