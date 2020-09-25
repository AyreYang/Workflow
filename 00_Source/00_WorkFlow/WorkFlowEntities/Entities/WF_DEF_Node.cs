using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using System.Linq;
using WorkFlow.Enums;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEntities.Entities
{
    [DBTableAttribute("WF_DEF_Nodes")]
    public class WF_DEF_Node : TableEntity, IDNode
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
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public int Type { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? RL_Approvers { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? RL_Input { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? RL_Output { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? RL_Status { get; set; }

        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Approvers { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Input { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Output { get; set; }
        [DBColumnAttribute(DBTYPE.UNIQID, false, true)]
        public Guid? CB_Notify { get; set; }


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

        [DBForeignAttribute("WorkflowID=>WorkflowID")]
        public DBRefList<WF_DEF_Callback> Callbacks { get; set; }
        [DBForeignAttribute("ID=>NodeID")]
        public DBRefList<WF_DEF_Rule> Rules { get; set; }
        [DBForeignAttribute("WorkflowID=>WorkflowID", "ID=>NodeID")]
        public DBRefList<WF_DEF_Mapping> Parents { get; set; }

        Guid[] IDNode.Parents => Parents.Entities.Select(e => e.ParentID).ToArray();

        public IDRule RL4Approvers => GetRule(RuleType.Rule4Approvers, this);

        public IDRule RL4Input => GetRule(RuleType.Rule4Input, this);

        public IDRule RL4Output => GetRule(RuleType.Rule4Output, this);

        public IDRule RL4Status => GetRule(RuleType.Rule4Status, this);

        public IDCallback CB4Approvers => GetCallback(CallbackType.Callback4Approvers, this);

        public IDCallback CB4Input => GetCallback(CallbackType.Callback4Input, this);

        public IDCallback CB4Output => GetCallback(CallbackType.Callback4Output, this);

        public IDCallback CB4Notify => GetCallback(CallbackType.Callback4Notify, this);
        
        private static IDRule GetRule(RuleType type, WF_DEF_Node node)
        {
            if (node == null) return null;

            IDRule rule = null;
            switch ((NodeType)node.Type)
            {
                case NodeType.Start:
                    switch (type)
                    {
                        case RuleType.Rule4Approvers:
                            rule = node.RL_Approvers.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Approvers.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Input:
                            rule = null;
                            break;
                        case RuleType.Rule4Output:
                            rule = node.RL_Output.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Output.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Status:
                            rule = node.RL_Status.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Status.Value && !r.IsDeleted) : null;
                            break;
                    }
                    break;
                case NodeType.Normal:
                    switch (type)
                    {
                        case RuleType.Rule4Approvers:
                            rule = node.RL_Approvers.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Approvers.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Input:
                            rule = node.RL_Input.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Input.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Output:
                            rule = node.RL_Output.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Output.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Status:
                            rule = node.RL_Status.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Status.Value && !r.IsDeleted) : null;
                            break;
                    }
                    break;
                case NodeType.Control:
                    switch (type)
                    {
                        case RuleType.Rule4Approvers:
                            rule = null;
                            break;
                        case RuleType.Rule4Input:
                            rule = node.RL_Input.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Input.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Output:
                            rule = node.RL_Output.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Output.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Status:
                            rule = null;
                            break;
                    }
                    break;
                case NodeType.End:
                    switch (type)
                    {
                        case RuleType.Rule4Approvers:
                            rule = null;
                            break;
                        case RuleType.Rule4Input:
                            rule = node.RL_Input.HasValue ? node.Rules.FirstOrDefault(r => r.Type == (int)type && r.ID == node.RL_Input.Value && !r.IsDeleted) : null;
                            break;
                        case RuleType.Rule4Output:
                            rule = null;
                            break;
                        case RuleType.Rule4Status:
                            rule = null;
                            break;
                    }
                    break;
            }

            return rule;
        }
        private static IDCallback GetCallback(CallbackType type, WF_DEF_Node node)
        {
            if (node == null) return null;

            IDCallback callback = null;
            switch ((NodeType)node.Type)
            {
                case NodeType.Start:
                    switch (type)
                    {
                        case CallbackType.Callback4Approvers:
                            callback = node.CB_Approvers.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Approvers.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Input:
                            callback = node.CB_Input.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Input.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Output:
                            callback = node.CB_Output.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Output.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Notify:
                            callback = node.CB_Notify.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Notify.Value && !cb.IsDeleted) : null;
                            break;
                    }
                    break;
                case NodeType.Normal:
                    switch (type)
                    {
                        case CallbackType.Callback4Approvers:
                            callback = node.CB_Approvers.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Approvers.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Input:
                            callback = node.CB_Input.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Input.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Output:
                            callback = node.CB_Output.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Output.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Notify:
                            callback = node.CB_Notify.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Notify.Value && !cb.IsDeleted) : null;
                            break;
                    }
                    break;
                case NodeType.Control:
                    switch (type)
                    {
                        case CallbackType.Callback4Approvers:
                            callback = null;
                            break;
                        case CallbackType.Callback4Input:
                            callback = node.CB_Input.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Input.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Output:
                            callback = node.CB_Output.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Output.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Notify:
                            callback = null;
                            break;
                    }
                    break;
                case NodeType.End:
                    switch (type)
                    {
                        case CallbackType.Callback4Approvers:
                            callback = null;
                            break;
                        case CallbackType.Callback4Input:
                            callback = node.CB_Input.HasValue ? node.Callbacks.FirstOrDefault(cb => cb.ID == node.CB_Input.Value && !cb.IsDeleted) : null;
                            break;
                        case CallbackType.Callback4Output:
                            callback = null;
                            break;
                        case CallbackType.Callback4Notify:
                            callback = null;
                            break;
                    }
                    break;
            }

            return callback;
        }
    }
}
