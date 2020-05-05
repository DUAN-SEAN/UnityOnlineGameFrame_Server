using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Crazy.ServerBase;

namespace GateServer
{
    /// <summary>
    /// Tank 玩家现场信息 目前为空 没有任何消息
    /// </summary>
    public partial class BDGameServerPlayerContext:PlayerContextBase
    {
        private static int AllocPlayerId = 1;

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
                        SendToInnerServer(ServerType.Game,iSyncBattleMessage);
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
        public override async void OnConnected()
        {
            base.OnConnected();

            m_gameUserId = "< "+GateServer.Instance.ServerId+" >Test< " + AllocPlayerId+++" >";
            bool flag = await OnVerifyIdentidy(m_gameUserId, m_gameUserId);


            Send(new S2C_AllocPlayerIdMessage {PlayerId = m_gameUserId });

            GateServer.Instance.PlayerCtxManager.RegisterPlayerContextByString(m_gameUserId, this);
        }
        /// <summary>
        /// 验证身份
        /// </summary>
        public async Task<bool> OnVerifyIdentidy(string username,string password)
        {
            //对玩家账号进行验证

            //验证成功后将username 和 随机化的 PlayerId 广播一个信息给Lobby服务
            int serverId =  await GateServer.Instance.CatchServer(ServerType.Lobby);
            //向该lobbyServer发送一条信息，表示该玩家来到了这个lobbyserver
            BindServer(ServerType.Lobby,serverId);

            return true;
        }
        
        public async Task OnAllocGameServer(long roomId)
        {
            

            var response = (L2G_GetRoomPlayersServerMessageACK)await CallToInnerServer(ServerType.Lobby, new G2L_GetRoomPlayersServerMessageReq {RoomId = roomId});
            

            var players = response.PlayerIds;

            var roomerId = response.RoomerId;

            if (roomerId == m_gameUserId)
            {
                int serverId = await GateServer.Instance.CatchServer(ServerType.Game);

                if (serverId < 1)
                {
                    Log.Info("OnAllocGameServer::CatchServer is Failed");
                    return;
                }
                BindServer(ServerType.Game, serverId);
            }
            else
            {
                BDGameServerPlayerContext context = GateServer.Instance.PlayerCtxManager.FindPlayerContextByString(roomerId) as BDGameServerPlayerContext;

                BindServer(ServerType.Game, context.m_currentGameServer);
            }
          
          

            return;

        }
        public void SendToInnerServer(ServerType serverType, IMessage message)
        {
            int serverId = -1;
            switch (serverType)
            {
                case ServerType.Game:
                    serverId = m_currentGameServer;
                    break;
                case ServerType.Lobby:
                    serverId = m_currentLobbyServer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }

            if (serverId == -1)
            {
                Log.Warning("SendToInnerServer::serverId = -1");
                return;
            }
            GateServer.Instance.SendInnerMessage(serverId,message);
        }
        public async Task<IResponse> CallToInnerServer(ServerType serverType, IRequest message)
        {
            int serverId = -1;
            switch (serverType)
            {
                case ServerType.Game:
                    serverId = m_currentGameServer;
                    break;
                case ServerType.Lobby:
                    serverId = m_currentLobbyServer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }

            if (serverId == -1)
            {
                Log.Warning("SendToInnerServer::serverId = -1");
                return null;
            }

            return await GateServer.Instance.CallInnerMessage(serverId, message);

        }


        private void BindServer(ServerType serverType, int serverId)
        {
            switch (serverType)
            {
                case ServerType.Game:
                    m_currentGameServer = serverId;
                    break;
                case ServerType.Lobby:
                    m_currentLobbyServer = serverId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }
        }
        public override Task OnDisconnected()
        {
            //向各个系统发送玩家离线消息

            //NetClientDisConnectMessage message = new NetClientDisConnectMessage{PlayerContextName = m_gameUserId};

            //GameServer.Instance.GetSystem<BattleSystem>().PostLocalMessage(message);
            //GameServer.Instance.GetSystem<LobbySystem>().PostLocalMessage(message);

            return base.OnDisconnected();

        }

        private int m_currentLobbyServer = -1;
        private int m_currentGameServer = -1;
    }
}
