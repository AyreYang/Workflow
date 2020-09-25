using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_DEF_Workflows")]
    public class WF_DEF_Workflow : TableEntity, IDWorkflow
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 100, false, false)]
        public string Code { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100, false, true)]
        public string Name { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, true)]
        public string Description { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Created { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Finished { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Error { get; set; }


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

        [DBForeignAttribute("ID=>WorkflowID")]
        public DBRefList<WF_DEF_Node> Nodes { get; set; }

        IDNode[] IDWorkflow.Nodes => Nodes.Entities;

        [DBForeignAttribute("ID=>WorkflowID")]
        public DBRefList<WF_DEF_Callback> Callbacks { get; set; }

        public IDCallback CB4Created => CB_Created.HasValue ? Callbacks.FirstOrDefault(e => e.ID.Equals(CB_Created.Value)) : null;

        public IDCallback CB4Finished => CB_Finished.HasValue ? Callbacks.FirstOrDefault(e => e.ID.Equals(CB_Finished.Value)) : null;

        public IDCallback CB4Error => CB_Error.HasValue ? Callbacks.FirstOrDefault(e => e.ID.Equals(CB_Error.Value)) : null;
    }
}
