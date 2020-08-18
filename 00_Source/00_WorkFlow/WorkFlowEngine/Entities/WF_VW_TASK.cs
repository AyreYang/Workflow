using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_VW_TASK", true)]
    public class WF_VW_TASK : TableEntity, ITask
    {
        public Guid Id
        {
            get { return string.IsNullOrEmpty(ID) ? Guid.Empty : Guid.Parse(ID); }
        }
        public Guid? BizId
        {
            get { return !string.IsNullOrEmpty(BizID) ? (Guid?)Guid.Parse(BizID) : null; }
        }
        public Guid WorkFlowId
        {
            get { return string.IsNullOrEmpty(WorkFlowID) ? Guid.Empty : Guid.Parse(WorkFlowID); }
        }
        public Guid? NodeId
        {
            get { return !string.IsNullOrEmpty(NodeID) ? (Guid?)Guid.Parse(NodeID) : null; }
        }

        public Guid? Pid
        {
            get { return !string.IsNullOrEmpty(PID) ? (Guid?)Guid.Parse(PID) : null; }
        }
        public Guid? Nid
        {
            get { return !string.IsNullOrEmpty(NID) ? (Guid?)Guid.Parse(NID) : null; }
        }

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string BizID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public string NID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public string PID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string WorkFlowID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string NodeID { get; set; }

        [DBColumnAttribute(DBTYPE.VARCHAR, 30, false, false)]
        public string WFCode { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long Status { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Comment { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Parameter { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
    }
}
