using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlow.Interfaces.Entities;

namespace WorkFlowEngine.Entities
{
    [DBTableAttribute("WF_ENT_WorkFlow")]
    public class WF_ENT_WorkFlow : TableEntity, IEWorkFlow
    {
        public Guid Id
        {
            get { return string.IsNullOrEmpty(ID) ? Guid.Empty : Guid.Parse(ID); }
            set { ID = Guid.Empty.Equals(value) ? null : value.ToString(); }
        }


        [DBColumnAttribute(DBTYPE.UNIQID, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100, false, false)]
        public string NameCN { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 100, false, false)]
        public string NameEN { get; set; }
        [DBColumnAttribute(DBTYPE.INT, false, false)]
        public long Version { get; set; }
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

        [DBForeignAttribute("ID=>WorkFlowID")]
        public DBRefList<WF_ENT_Node> Nodes { get; set; }

        public IList<WF_ENT_NodeMapping> Mappings
        {
            get
            {
                var list = new List<WF_ENT_NodeMapping>();
                foreach (var ent in Nodes.Entities)
                {
                    list.AddRange(ent.Mappings.Entities);
                }
                return list;
            }
        }

        string IEWorkFlow.Name => string.IsNullOrWhiteSpace(this.NameCN) ? this.NameEN : this.NameCN;

        IENode[] IEWorkFlow.Nodes => this.Nodes != null ? this.Nodes.Entities.Select(e => e as IENode).ToArray() : new List<IENode>().ToArray();
    }
}
