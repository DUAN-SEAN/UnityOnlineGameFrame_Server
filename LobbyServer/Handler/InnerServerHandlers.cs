using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.System;

namespace LobbyServer
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
            reply(new S2G_CatchServerMessage{ServerId = LobbyServer.Instance.ServerId});
        }
    }
    /// <summary>
    /// 网关服务需要获取房间内玩家列表 用于绑定GameServer
    /// </summary>
    [MessageHandler]
    public class G2L_GetRoomPlayersServerMessageHandler: AMRpcHandler<G2L_GetRoomPlayersServerMessageReq,L2G_GetRoomPlayersServerMessageACK>
    {
        protected override void Run(ISession playerContext, G2L_GetRoomPlayersServerMessageReq message, Action<L2G_GetRoomPlayersServerMessageACK> reply)
        {
            Log.Info("GetRoomPlayersServerMessage "+message.RoomId);
            var resposne = new L2G_GetRoomPlayersServerMessageACK();
            var roomId = message.RoomId;
            var roomItem = LobbyServer.Instance.GetSystem<LobbySystem>().GetRoomItem(roomId);
            foreach (var t in roomItem.Players)
            {
                resposne.PlayerIds.Add(t);
            }

            resposne.RoomerId = roomItem.Roomer;

            reply(resposne);
        }
    }
}
