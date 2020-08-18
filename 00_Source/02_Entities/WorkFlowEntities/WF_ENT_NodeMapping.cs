using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;

namespace WorkFlowEntities
{
    [DBTableAttribute("WF_ENT_NodeRule")]
    public class WF_ENT_NodeMapping : TableEntity
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
        public Guid ParentId
        {
            get { return string.IsNullOrEmpty(ParentID) ? Guid.Empty : Guid.Parse(ParentID); }
            set { ParentID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string NodeID { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string ParentID { get; set; }
        [DBColumnAttribute(DBTYPE.BOOLEAN, false, false, false)]
        public bool IsDeleted { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }
    }
}
