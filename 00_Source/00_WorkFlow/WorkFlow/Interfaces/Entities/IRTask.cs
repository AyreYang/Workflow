using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interfaces.Entities
{
    public interface IRTask
    {
        Guid ID { get; }
        Guid? DetailID { get; }

        Guid? NodeID { get; }   //通过DetailID关联出NodeID
        Guid? InstID { get; }   //通过NodeID关联出InstID

        Guid WorkflowID { get; }
        string BizCode { get; }

        int Action { get; }
        string Comment { get; }
        string Parameter { get; }
        dynamic Variables { get; }  //Json反序列化Parameter

        string CreatedBy { get; }
        DateTime CreatedOn { get; }


    }
}
