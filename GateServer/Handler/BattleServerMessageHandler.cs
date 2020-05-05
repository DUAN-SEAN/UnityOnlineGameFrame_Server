using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.ServerBase;
using GateServer;

namespace GateServer.Handler
{
    //[MessageHandler]
    //public class C2S_PlayerSyncEntityMessageHandler:AMHandler<C2S_PlayerSyncEntityMessage>
    //{
    //    protected override void Run(ISession playerContext, C2S_PlayerSyncEntityMessage message)
    //    {
    //        //流入BattleSystem
           
    //    }
    //}
    [MessageHandler]
    public class C2B_EnterClubBattleMessageHandler:AMRpcHandler<C2B_EnterClubBattleReqMessage,B2C_EnterClubBattleAckMessage>
    {
        protected override async void Run(ISession playerContext, C2B_EnterClubBattleReqMessage message, Action<B2C_EnterClubBattleAckMessage> reply)
        {


            BDGameServerPlayerContext context = playerContext as BDGameServerPlayerContext;

            
            var response = (B2C_EnterClubBattleAckMessage) await context.CallToInnerServer(ServerType.Game, message);
            reply(response);
        }
    }
    [MessageHandler]
    public class C2G_AllocBattleServerMessage:AMRpcHandler<C2G_AllocBattleServerMessageReq,C2G_AllocBattleServerMessageAck>
    {
      

        protected override async void Run(ISession playerContext, C2G_AllocBattleServerMessageReq message, Action<C2G_AllocBattleServerMessageAck> reply)
        {
            try
            {
                BDGameServerPlayerContext context = playerContext as BDGameServerPlayerContext;

                await context.OnAllocGameServer(message.RoomId);
                //if(flag) Log.Info("C2G_AllocBattleServerMessage::OnAllocGameServer Succeed!");
                reply(new C2G_AllocBattleServerMessageAck { PlayerId = message.PlayerId });
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
          
        }
    }
}
