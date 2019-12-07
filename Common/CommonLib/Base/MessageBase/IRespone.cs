using System;
using System.Collections.Generic;
using System.Text;

namespace Crazy.Common
{
    /// <summary>
    /// RPC相应消息
    /// </summary>
    public interface IResponse : IMessage
    {
        int Error { get; set; }
        string Message { get; set; }
        int RpcId { get; set; }
    }
}
