using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.System;

namespace GameServer.Handler
{
    [MessageHandler]
    public class C2S_PlayerSyncEntityMessageHandler:AMHandler<C2S_PlayerSyncEntityMessage>
    {
        protected override void Run(ISession playerContext, C2S_PlayerSyncEntityMessage message)
        {
            //流入BattleSystem
            GameServer.Instance.GetSystem<BattleSystem>().PostLocalMessage(new PlayerSyncEntityBattleSystemMessage
                {PlayerSyncEntityMessage = message});
        }
    }
}
