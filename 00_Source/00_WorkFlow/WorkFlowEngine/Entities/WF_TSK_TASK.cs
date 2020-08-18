using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_TSK_TASK")]
    public class WF_TSK_TASK : TableEntity
    {
        public Guid Id
        {
            get { return string.IsNullOrEmpty(ID) ? Guid.Empty : Guid.Parse(ID); }
            set { ID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }
        public Guid? BizId
        {
            get { return !string.IsNullOrEmpty(BizID) ? (Guid?)Guid.Parse(BizID) : null; }
            set { BizID = value == null ? null : value.ToString(); }
        }
        public Guid WorkFlowId
        {
            get { return string.IsNullOrEmpty(WorkFlowID) ? Guid.Empty : Guid.Parse(WorkFlowID); }
            set { WorkFlowID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string BizID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string WorkFlowID { get; set; }
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
