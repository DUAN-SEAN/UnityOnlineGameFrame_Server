using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.ServerBase;
using GateServer;
using Google.Protobuf.Collections;

namespace GameServer.Handler
{
    [MessageHandler]
    public class C2L_CreateRoomReqMessageHandler:AMRpcHandler<C2L_CreateRoomReqMessage,L2C_CreateRoomAckMessage>
    {
        protected override async void Run(ISession playerContext, C2L_CreateRoomReqMessage message, Action<L2C_CreateRoomAckMessage> reply)
        {
            BDGameServerPlayerContext context = playerContext as BDGameServerPlayerContext;
            var response = await context.CallToInnerServer(ServerType.Lobby, message) as L2C_CreateRoomAckMessage;

            reply(response);

        }
    }
    [MessageHandler]
    public class C2L_JoinRoomReqMessageHandler:AMRpcHandler<C2L_JoinRoomReqMessage,L2C_JoinRoomAckMessage>
    {
        protected override async void Run(ISession playerContext, C2L_JoinRoomReqMessage message, Action<L2C_JoinRoomAckMessage> reply)
        {
            BDGameServerPlayerContext context = playerContext as BDGameServerPlayerContext;
            var response = await context.CallToInnerServer(ServerType.Lobby, message) as L2C_JoinRoomAckMessage;

            reply(response);

        }
    }
    [MessageHandler]
    public class C2L_GetRoomInfoReqMessageHandler:AMRpcHandler<C2L_GetRoomInfoReqMessage,L2C_GetRoomInfoAckMessage>
    {
        protected override async void Run(ISession playerContext, C2L_GetRoomInfoReqMessage message, Action<L2C_GetRoomInfoAckMessage> reply)
        {
          

            BDGameServerPlayerContext context = playerContext as BDGameServerPlayerContext;
            var response = await context.CallToInnerServer(ServerType.Lobby, message) as L2C_GetRoomInfoAckMessage;

            reply(response);
        }
    }
    [MessageHandler]
    public class C2L_GetRoomListInfoReqMessageHandler : AMRpcHandler<C2L_GetRoomListInfoReqMessage, L2C_GetRoomListInfoAckMessage>
    {
        protected override async void Run(ISession playerContext, C2L_GetRoomListInfoReqMessage message, Action<L2C_GetRoomListInfoAckMessage> reply)
        {
            BDGameServerPlayerContext context = playerContext as BDGameServerPlayerContext;
            var response = await context.CallToInnerServer(ServerType.Lobby, message) as L2C_GetRoomListInfoAckMessage;


            reply(response);
        }
    }
}
