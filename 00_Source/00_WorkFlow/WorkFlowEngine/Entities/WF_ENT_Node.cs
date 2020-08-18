using Database.Commons.Objects;
using Database.Commons.Objects.SQLItems;
using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Components;
using WorkFlow.Enums;
using WorkFlow.Interfaces;
using WorkFlow.Interfaces.Entities;
using WorkFlowEngine.Rules;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_ENT_Node")]
    public class WF_ENT_Node : TableEntity, IENode
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
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long Type { get; set; }
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


        NodeType IENode.Type => (new long[] { 0, 1, 2 }).Any(l => l == this.Type) ? (NodeType)this.Type : NodeType.UNKNOWN;

        Guid[] IENode.Parents => Mappings != null ? Mappings.Entities.Select(e => e.ParentId).ToArray() : new List<Guid>().ToArray();


        protected override Clause Filter(TableItem item, SQLParamCreater creater)
        {
            return item.ColumnEquals("IsDeleted", creater.NewParameter(false));
        }

        #region rules
        private Func<INode[], bool> _rule4Input = null;
        public Func<INode[], bool> RULE4Input
        {
            get
            {
                if(_rule4Input == null)
                {
                    var rule = this.Rules.Entities.FirstOrDefault(r => r.RuleType == 1);
                    _rule4Input = rule != null ? RuleFactory.Inst.FetchRule4Input(this.WorkFlowId, (int)rule.ProcType, rule.Content) : InputRules.DefaultRule;
                }
                return _rule4Input;
            }
        }

        private Func<IPNodeDetail[], INode[], OPResult> _rule4Output = null;
        public Func<IPNodeDetail[], INode[], OPResult> RULE4Output
        {
            get
            {
                if (_rule4Output == null)
                {
                    var rule = this.Rules.Entities.FirstOrDefault(r => r.RuleType == 2);
                    _rule4Output = rule != null ? RuleFactory.Inst.FetchRule4Output(this.WorkFlowId, (int)rule.ProcType, rule.Content) : OutputRules.DefaultRule0;
                }
                return _rule4Output;
            }
        }

        private Func<KeyValuePair<string, object>[], string[]> _rule4Approvers = null;
        public Func<KeyValuePair<string, object>[], string[]> RULE4Approvers
        {
            get
            {
                if (this.Type == (long)NodeType.Approval && _rule4Approvers == null)
                {
                    var rule = this.Rules.Entities.FirstOrDefault(r => r.RuleType == 0);
                    _rule4Approvers = rule != null ? RuleFactory.Inst.FetchRule4Approvers(this.WorkFlowId, (int)rule.ProcType, rule.Content) : null;
                }
                return _rule4Approvers;
            }
        }
        #endregion
    }
}
