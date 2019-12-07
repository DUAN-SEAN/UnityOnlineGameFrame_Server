using System;
using System.Collections.Generic;
using System.Text;

namespace Crazy.Common
{
    /// <summary>
    /// RPC请求消息
    /// </summary>
    public interface IRequest : IMessage
    {
        int RpcId { get; set; }
    }
}
