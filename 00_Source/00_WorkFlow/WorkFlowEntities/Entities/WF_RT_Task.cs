using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using Newtonsoft.Json;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_RT_Tasks")]
    public class WF_RT_Task : TableEntity, IRTask
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? DetailID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, false)]
        public Guid WorkflowID { get; set; }

        [DBColumnAttribute(DBTYPE.VARCHAR, 30, false, true)]
        public string BizCode { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int Action { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, true)]
        public string Comment { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, true)]
        public string Parameter { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 255)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }

        public Guid? NodeID { get; set; }

        public Guid? InstID { get; set; }

        public dynamic Variables => !string.IsNullOrWhiteSpace(Parameter) ? JsonConvert.DeserializeObject(Parameter) : null;
    }
}
