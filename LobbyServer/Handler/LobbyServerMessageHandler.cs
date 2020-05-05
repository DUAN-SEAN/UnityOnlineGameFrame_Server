using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.System;
using Google.Protobuf.Collections;

namespace LobbyServer
{
    [MessageHandler]
    public class C2L_CreateRoomReqMessageHandler:AMRpcHandler<C2L_CreateRoomReqMessage,L2C_CreateRoomAckMessage>
    {
        protected override void Run(ISession playerContext, C2L_CreateRoomReqMessage message, Action<L2C_CreateRoomAckMessage> reply)
        {
            var response = new L2C_CreateRoomAckMessage();
            var item = LobbyServer.Instance.GetSystem<LobbySystem>().OnCreateRoom(message.PlayerId);

            response.RoomId = item.RoomId;

            reply(response);

        }
    }
    [MessageHandler]
    public class C2L_JoinRoomReqMessageHandler:AMRpcHandler<C2L_JoinRoomReqMessage,L2C_JoinRoomAckMessage>
    {
        protected override void Run(ISession playerContext, C2L_JoinRoomReqMessage message, Action<L2C_JoinRoomAckMessage> reply)
        {
            var response = new L2C_JoinRoomAckMessage();

            var item = LobbyServer.Instance.GetSystem<LobbySystem>().OnJoinRoom(message.PlayerId,message.RoomId);

            response.RoomId = item.RoomId;

            reply(response);

        }
    }
    [MessageHandler]
    public class C2L_GetRoomInfoReqMessageHandler:AMRpcHandler<C2L_GetRoomInfoReqMessage,L2C_GetRoomInfoAckMessage>
    {
        protected override void Run(ISession playerContext, C2L_GetRoomInfoReqMessage message, Action<L2C_GetRoomInfoAckMessage> reply)
        {
            var response = new L2C_GetRoomInfoAckMessage();

            var roomItem = LobbyServer.Instance.GetSystem<LobbySystem>().GetRoomItem(message.RoomId);
            response.RoomInfo = new RoomInfoMessage();
            if (roomItem == null)
                response.RoomInfo.RoomId = -1L;
            else
            {
              
                response.RoomInfo.RoomId = roomItem.RoomId;
                foreach (var id in roomItem.Players)
                {
                    response.RoomInfo.PlayerIds.Add(id);
                }
                response.RoomInfo.RoomCapacity = roomItem.Capacity;
                response.RoomInfo.RoomName = roomItem.RoomName;
                response.RoomInfo.Roomer = roomItem.Roomer;
                response.RoomInfo.RoomState = roomItem.RoomState;
            }

            reply(response);
        }
    }
    [MessageHandler]
    public class C2L_GetRoomListInfoReqMessageHandler : AMRpcHandler<C2L_GetRoomListInfoReqMessage, L2C_GetRoomListInfoAckMessage>
    {
        protected override void Run(ISession playerContext, C2L_GetRoomListInfoReqMessage message, Action<L2C_GetRoomListInfoAckMessage> reply)
        {
            try
            {
                var response = new L2C_GetRoomListInfoAckMessage();

                var roomItems = LobbyServer.Instance.GetSystem<LobbySystem>().GetRoomList();
                if (roomItems == null || roomItems.Count == 0)
                {
                }

                else
                {
                    foreach (var roomItem in roomItems)
                    {
                        var item = new RoomInfoMessage
                        {
                            RoomId = roomItem.RoomId,
                            RoomCapacity = roomItem.Capacity,
                            RoomName = roomItem.RoomName,
                            Roomer = roomItem.Roomer,
                            RoomState = roomItem.RoomState
                        };
                        foreach (var id in roomItem.Players)
                        {
                            item.PlayerIds.Add(id);
                        }

                        response.RoomInfos.Add(item);
                    }

                }

                Log.Info("C2L_GetRoomListInfoReqMessageHandler::Return response RpcId = " + response.RpcId);
                reply(response);
            }
            catch (Exception e)
            {
                Log.Info("C2L_GetRoomListInfoReqMessageHandler"+e.ToString());
            }
            
        }
    }
}
