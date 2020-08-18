using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Enums;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_PRC_NodeDetail")]
    public class WF_PRC_NodeDetail : TableEntity, IPNodeDetail
    {
        public Guid Id
        {
            get { return string.IsNullOrEmpty(ID) ? Guid.Empty : Guid.Parse(ID); }
            set { ID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }
        public Guid Nid
        {
            get { return string.IsNullOrEmpty(NID) ? Guid.Empty : Guid.Parse(NID); }
            set { NID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string NID { get; set; }

        [DBColumnAttribute(DBTYPE.VARCHAR, 100, false, false)]
        public string UserID { get; set; }

        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long Status { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Comment { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Parameter { get; set; }

        [DBColumnAttribute(DBTYPE.BOOLEAN, false, false, true)]
        public bool Enabled { get; set; }

        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string CreatedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime CreatedOn { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 50)]
        public string LastModifiedBy { get; set; }
        [DBColumnAttribute(DBTYPE.DATETIME, false, false, DBColumnDefaultValue.CURRENT_TIME)]
        public DateTime LastModifiedOn { get; set; }

        NodeStatus IPNodeDetail.Status => (NodeStatus)Status;

        protected override Clause Filter(TableItem item, SQLParamCreater creater)
        {
            return item.ColumnEquals("Enabled", creater.NewParameter(true));
        }
    }
}
