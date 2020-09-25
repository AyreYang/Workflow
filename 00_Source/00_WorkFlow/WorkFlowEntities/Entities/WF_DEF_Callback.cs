using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_DEF_Callbacks")]
    public class WF_DEF_Callback : TableEntity, IDCallback
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid WorkflowID { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 100, false, false)]
        public string Code { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100, false, true)]
        public string Name { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, true)]
        public string Description { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Url { get; set; }

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
