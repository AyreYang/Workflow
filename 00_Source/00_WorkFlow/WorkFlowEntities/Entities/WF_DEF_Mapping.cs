using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_DEF_Mappings")]
    public class WF_DEF_Mapping : TableEntity, IDMapping
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid WorkflowID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid NodeID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid ParentID { get; set; }

        [DBColumnAttribute(DBTYPE.BOOLEAN, false, false, false)]
        public bool IsDeleted { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }
    }
}
