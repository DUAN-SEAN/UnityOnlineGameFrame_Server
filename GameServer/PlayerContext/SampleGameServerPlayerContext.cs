using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.ServerBase;

namespace GameServer
{
    /// <summary>
    /// Tank 玩家现场信息 目前为空 没有任何消息
    /// </summary>
    public partial class SampleGameServerPlayerContext:PlayerContextBase
    {
        private static int AllocPlayerId = 1;
        public override void OnConnected()
        {
            base.OnConnected();

            m_gameUserId = "test " + AllocPlayerId++;

            Send(new S2C_AllocPlayerIdMessage {PlayerId = m_gameUserId });

            GameServer.Instance.PlayerCtxManager.RegisterPlayerContextByString(m_gameUserId, this);
        }
    }
}
