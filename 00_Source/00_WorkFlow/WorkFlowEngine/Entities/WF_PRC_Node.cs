using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_PRC_Node")]
    public class WF_PRC_Node : TableEntity, IPNode
    {
        public Guid Id
        {
            get { return string.IsNullOrEmpty(ID) ? Guid.Empty : Guid.Parse(ID); }
            set { ID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }
        public Guid NodeId
        {
            get { return string.IsNullOrEmpty(NodeID) ? Guid.Empty : Guid.Parse(NodeID); }
            set { NodeID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }
        public Guid Pid
        {
            get { return string.IsNullOrEmpty(PID) ? Guid.Empty : Guid.Parse(PID); }
            set { PID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string NodeID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string PID { get; set; }


        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long SEQ { get; set; }

        public long Seq => this.SEQ;

        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long RoundNO { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long Status { get; set; }




        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }

        [DBForeignAttribute("ID=>NID")]
        public DBRefList<WF_PRC_NodeDetail> Details { get; set; }

        IList<IPNodeDetail> IPNode.Details => Details.Entities.Select(e => e as IPNodeDetail).ToArray();

        int IPNode.Status => (int)this.Status;
    }
}
