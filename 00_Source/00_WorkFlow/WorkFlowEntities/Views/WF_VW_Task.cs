using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using Newtonsoft.Json;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Views
{
    [DBTableAttribute("WF_VW_Tasks", true)]
    public class WF_VW_Task : TableEntity, IRTask
    {
        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public Guid ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public Guid? DetailID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? InstID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? NodeID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public Guid WorkflowID { get; set; }

        [DBColumnAttribute(DBTYPE.VARCHAR, 30, false, true)]
        public string BizCode { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int Action { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Comment { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Parameter { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }



        public dynamic Variables => !string.IsNullOrWhiteSpace(Parameter) ? JsonConvert.DeserializeObject(Parameter) : null;

    }
}
