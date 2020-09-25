using System;

namespace WorkFlow.Interfaces.Entities
{
    public interface IRNode
    {
        Guid ID { get; }
        Guid NodeID { get; }
        Guid InstID { get; }
        int SEQ { get; }
        int RoundNO { get; }
        int Status { get; }
        int? Status0 { get; set; }  //节点状态缓存

        IRDetail[] Details { get; }
    }
}
