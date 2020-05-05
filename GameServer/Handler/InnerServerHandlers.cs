using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.System;

namespace GameServer
{
    [MessageHandler]
    public class TestServerMessageHandler:AMHandler<G2S_CatchServerMessage>
    {
        protected override void Run(ISession playerContext, G2S_CatchServerMessage message)
        {
            Log.Info("TestServerMessageHandler::ServerId = "+message.ServerId);
        }
    }


    [MessageHandler]
    public class G2L_CatchServerMessageHandler : AMRpcHandler<G2S_CatchServerMessage, S2G_CatchServerMessage>
    {
        protected override void Run(ISession playerContext, G2S_CatchServerMessage message, Action<S2G_CatchServerMessage> reply)
        {
            Log.Info("CatchServer::ServerId"+ message.ServerId); 
            reply(new S2G_CatchServerMessage{ServerId = GameServer.Instance.ServerId});
        }
    }
 
}
