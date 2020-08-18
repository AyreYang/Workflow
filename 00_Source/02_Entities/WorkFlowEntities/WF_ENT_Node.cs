using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;

namespace WorkFlowEntities
{
    [DBTableAttribute("WF_ENT_Node")]
    public class WF_ENT_Node : TableEntity
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

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string WorkFlowID { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100, false, false)]
        public string NameCN { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 100, false, false)]
        public string NameEN { get; set; }

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

        [DBForeignAttribute("ID=>NodeID")]
        public DBRefList<WF_ENT_NodeRule> Rules { get; set; }
        [DBForeignAttribute("ID=>NodeID")]
        public DBRefList<WF_ENT_NodeMapping> Mappings { get; set; }

    }
}
