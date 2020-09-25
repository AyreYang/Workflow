using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_RT_Details")]
    public class WF_RT_Detail : TableEntity, IRDetail
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid NodeID { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100, false, false)]
        public string UserID { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int Status { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, true)]
        public string Comment { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, true)]
        public string Parameter { get; set; }

        [DBColumnAttribute(DBTYPE.BOOLEAN, false, false, true)]
        public bool Enabled { get; set; }
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
