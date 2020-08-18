using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_PRC_WorkFlow")]
    public class WF_PRC_WorkFlow : TableEntity, IPWorkFlow
    {
        public Guid Id
        {
            get { return string.IsNullOrEmpty(ID) ? Guid.Empty : Guid.Parse(ID); }
            set { ID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }
        public Guid WorkFlowId
        {
            get { return string.IsNullOrEmpty(WorkFlowID) ? Guid.Empty : Guid.Parse(WorkFlowID); }
            set { WorkFlowID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }
        public Guid? TaskId
        {
            get { return !string.IsNullOrEmpty(TaskID) ? (Guid?)Guid.Parse(TaskID) : null; }
            set { TaskID = value.HasValue ? value.Value.ToString() : null; }
        }

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string WorkFlowID { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 30, false, false)]
        public string WFCode { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long Status { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string OwnerID { get; set; }
        [DBColumnAttribute(DBTYPE.BOOLEAN, false, false)]
        public bool ProcStatus { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true, null)]
        public string TaskID { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }

        [DBForeignAttribute("ID=>PID")]
        public DBRefList<WF_PRC_Node> Nodes { get; set; }

        IList<IPNode> IPWorkFlow.Nodes => Nodes.Entities.Select(e => e as IPNode).ToArray();

        int IPWorkFlow.Status => (int)this.Status;

        string IPWorkFlow.OwnerID => this.OwnerID;
    }
}
