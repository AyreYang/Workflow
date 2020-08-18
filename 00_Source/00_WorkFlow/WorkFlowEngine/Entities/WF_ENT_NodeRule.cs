using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_ENT_NodeRule")]
    public class WF_ENT_NodeRule : TableEntity, IERule
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

        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID)]
        public string NodeID { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long RuleType { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long ProcType { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100, false, false)]
        public string NameCN { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 100, false, false)]
        public string NameEN { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 9999, false, false)]
        public string Content { get; set; }

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

        public string Name => this.NameEN;

        int IERule.RuleType => (int)this.RuleType;

        int IERule.ProcType => (int)this.ProcType;

        protected override Clause Filter(TableItem item, SQLParamCreater creater)
        {
            return item.ColumnEquals("IsDeleted", creater.NewParameter(false));
        }
    }
}
