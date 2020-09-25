using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IRDetail
    {
        Guid ID { get; }
        Guid NodeID { get; }
        int Status { get; }
        string UserID { get; }

        string Comment { get; }
        string Parameter { get; }

        bool Enabled { get; }

        //string LastModifiedBy { get; }
        //DateTime LastModifiedOn { get; }
    }
}
