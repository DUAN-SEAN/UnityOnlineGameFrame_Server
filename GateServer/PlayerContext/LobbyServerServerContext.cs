using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Crazy.ServerBase;

namespace GameServer
{
    /// <summary>
    /// 对GameServer 的ServerContext
    /// </summary>
    public partial class LobbyServerServerContext : ServerContextBase
    {
        public LobbyServerServerContext(int id, ServerType serverType) : base(id, serverType)
        {
        }
        public override Task OnMessage(ILocalMessage msg)
        {
            //switch (msg.MessageId)
            //{
              
            //    case ServerBaseLocalMesssageIDDef.NetMessage://捕获一下网络普通消息，如果是同步战斗消息，则直接交给BattleSystem去转发
            //        NetClientMessage message = msg as NetClientMessage;
            //        // ReSharper disable once UsePatternMatching
            //        var iSyncBattleMessage = message.MessageInfo.Message as ISyncBattleMessage;
            //        if (iSyncBattleMessage != null)
            //        {
                        
                        
            //            GameServer.Instance.GetSystem<BattleSystem>().PostLocalMessage(new SyncBattleMsgSystemMessage
            //                { PlayerSyncEntityMessage = iSyncBattleMessage });
            //            return Task.CompletedTask;
            //        }
            //        break;
            //    default: break;
            //}

            return base.OnMessage(msg);


        }

        /// <summary>
        /// 目前做个测试，给每个玩家随机分配一个用户名
        /// </summary>
        public override void OnConnected()
        {
            //Send(new G2S_TestServerMessage {ServerId = GateServer.GateServer.Instance.ServerId});
        }

        public override Task OnDisconnected()
        {
            

            return base.OnDisconnected();

        }
        public async Task<int> CatchServer()
        {

            var message = (S2G_CatchServerMessage)await Call(new G2S_CatchServerMessage { ServerId = GateServer.GateServer.Instance.ServerId });


            return message.ServerId;
        }


    }
}
