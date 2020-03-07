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
    [MessageHandler]
    public class C2B_EnterClubBattleMessageHandler:AMRpcHandler<C2B_EnterClubBattleReqMessage,B2C_EnterClubBattleAckMessage>
    {
        protected override void Run(ISession playerContext, C2B_EnterClubBattleReqMessage message, Action<B2C_EnterClubBattleAckMessage> reply)
        {
            var response = new B2C_EnterClubBattleAckMessage();
            var flag = GameServer.Instance.GetSystem<BattleSystem>()
                .EnterClubBattle((playerContext as SampleGameServerPlayerContext).ContextStringName, message.RoomId);
            if (flag) response.RoomId = message.RoomId;
            else response.RoomId = -1;
            
            reply(response);
        }
    }
}
