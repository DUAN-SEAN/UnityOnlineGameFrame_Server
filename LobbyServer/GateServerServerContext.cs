using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Crazy.ServerBase;

namespace LobbyServer
{
    /// <summary>
    /// 对GameServer 的ServerContext
    /// </summary>
    public partial class GateServerServerContext : PlayerContextBase
    {
        /// <summary>
        /// 目前做个测试，给每个玩家随机分配一个用户名
        /// </summary>
        public override void OnConnected()
        {
            Log.Info("LobbyServer Id = "+LobbyServer.Instance.ServerId+"Connected Gate");
        }

        public override Task OnDisconnected()
        {
            //向各个系统发送玩家离线消息
            //NetClientDisConnectMessage message = new NetClientDisConnectMessage{PlayerContextName = m_gameUserId};

            //GameServer.Instance.GetSystem<BattleSystem>().PostLocalMessage(message);
            //GameServer.Instance.GetSystem<LobbySystem>().PostLocalMessage(message);

            return base.OnDisconnected();

        }
        /// <summary>
        /// 将玩家注册到这个服务器上
        /// 
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool OnRegisterPlayer(int playerId,string userName)
        {
            return true;
        }
    }
}
