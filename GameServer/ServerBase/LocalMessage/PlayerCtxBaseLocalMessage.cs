using Crazy.Common;
using Crazy.NetSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.ServerBase
{
    /// <summary>
    /// 装载着所有本地逻辑消息
    /// </summary>



    public class NetClientMessage : ILocalMessage
    {
        public int MessageId => ServerBaseLocalMesssageIDDef.NetMessage;

        public MessageInfo MessageInfo;
    }

    public class RpcNetClientMessage : ILocalMessage
    {
        public int MessageId => ServerBaseLocalMesssageIDDef.RpcNetMessage;

        public MessageInfo MessageInfo;
    }

    public class SystemSendNetMessage : ILocalMessage
    {
        public int MessageId => ServerBaseLocalMesssageIDDef.SystemSendNetMessage;
        /// <summary>
        /// 玩家应用层Id
        /// </summary>
        public string PlayerId;
        /// <summary>
        /// 待发送的网络消息
        /// </summary>
        public IMessage Message;
    }

    public class NetClientDisConnectMessage : ILocalMessage
    {
        public int MessageId { get => ServerBaseLocalMesssageIDDef.NetClientDisConnectMessageDef; }

        public string PlayerContextName;
    }
}
