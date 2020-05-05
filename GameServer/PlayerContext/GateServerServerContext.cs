using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Crazy.ServerBase;
using GameServer.System;

namespace GameServer
{
    /// <summary>
    /// 对GameServer 的ServerContext
    /// </summary>
    public partial class GateServerServerContext : PlayerContextBase
    {

        public override Task OnMessage(ILocalMessage msg)
        {
            switch (msg.MessageId)
            {

                case ServerBaseLocalMesssageIDDef.NetMessage://捕获一下网络普通消息，如果是同步战斗消息，则直接交给BattleSystem去转发
                    NetClientMessage message = msg as NetClientMessage;
                    // ReSharper disable once UsePatternMatching
                    var iSyncBattleMessage = message.MessageInfo.Message as ISyncBattleMessage;
                    if (iSyncBattleMessage != null)
                    {


                        GameServer.Instance.GetSystem<BattleSystem>().PostLocalMessage(new SyncBattleMsgSystemMessage
                        { PlayerSyncEntityMessage = iSyncBattleMessage });
                        return Task.CompletedTask;
                    }
                    break;
                default: break;
            }

            return base.OnMessage(msg);


        }

        /// <summary>
        /// 目前做个测试，给每个玩家随机分配一个用户名
        /// </summary>
        public override void OnConnected()
        {
           
        }

        public override Task OnDisconnected()
        {
            //向各个系统发送玩家离线消息
            //NetClientDisConnectMessage message = new NetClientDisConnectMessage{PlayerContextName = m_gameUserId};

            //GameServer.Instance.GetSystem<BattleSystem>().PostLocalMessage(message);
            //GameServer.Instance.GetSystem<LobbySystem>().PostLocalMessage(message);

            return base.OnDisconnected();

        }
    }
}
