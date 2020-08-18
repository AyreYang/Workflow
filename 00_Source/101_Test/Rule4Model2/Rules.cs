using Database.Entity;
using Database.Entity.Attributes;
using Database.Entity.Enums;
using Database.Implements.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rule4Model2
{
    public class Rules
    {
        public static string[] FindApprovers(KeyValuePair<string, object>[] parameters)
        {
            string dbcnn = @"E:\01_Workspace\01_VS\05_WorkFlow\99_Temp\WorkFlow.s3db";
            var accessor = new DatabaseAccessor(dbcnn);
            var dbcontext = new DBContext(accessor);
            var list = dbcontext.Retrieve<WF_MST_Users>();
            return list.Entities.Select(e => e.ID).ToArray();
        }
    }

    [DBTableAttribute("WF_MST_Users", true)]
    public class WF_MST_Users : TableEntity
    {
        [DBColumnAttribute(DBTYPE.VARCHAR, 100, true)]
        public string ID { get; set; }
        [DBColumnAttribute(DBTYPE.NVARCHAR, 100)]
        public string NameCN { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 100)]
        public string NameEN { get; set; }
        [DBColumnAttribute(DBTYPE.INT)]
        public long Gender { get; set; }
        [DBColumnAttribute(DBTYPE.VARCHAR, 500)]
        public string Email { get; set; }
    }
}
