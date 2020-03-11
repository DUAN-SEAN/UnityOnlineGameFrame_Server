using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.ServerBase
{
    public class ServerBaseLocalMesssageIDDef
    {
        public const Int32 LocalMsgAsyncActionResult = 2;

        public const Int32 NetMessage = 3;

        public const Int32 RpcNetMessage = 4;

        public const Int32 LocalMsgPlayCtxTimer = 5;
        /// <summary>
        /// 服务器各系统要求玩家现场发送消息给客户端
        /// </summary>
        public const Int32 SystemSendNetMessage = 6;
        //从1002开始由逻辑业务控制

        public const Int32 NetClientDisConnectMessageDef = 1001;

    }

}
