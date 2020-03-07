using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.NetSharp;
using Crazy.ServerBase;
using GameServer.System.SystemLocalMessage.Lobby;
using NLog.Fluent;

namespace GameServer.System
{
    public class LobbySystem:BaseSystem
    {
        public override Task OnMessage(ILocalMessage msg)
        {

            switch (msg.MessageId)
            {
                case LobbySystemLocalMessageIDDef.CreateRoomReqLocalMessageID:
                    OnCreateRoom((msg as CreateRoomReqLocalMessage).PlayerId);


                    break;

                default:
                    break;
            }

            return base.OnMessage(msg);


        }

        public override void Update(int data1 = 0, long data2 = 0, object data3 = null)
        {
            base.Update(data1, data2, data3);

            TickRoomState();

        }

        private void TickRoomState()
        {
            foreach (var idAndItem in m_RoomItemDic)
            {
                
            }
        }
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public RoomItem OnCreateRoom(string playerId)
        {
            RoomItem roomItem = new RoomItem();
            var state = VerifyPlayerState(playerId);
            if (state != 0)
            {
                roomItem.RoomId = -1;
                return roomItem;
            }

            roomItem = new RoomItem
            {
                Capacity = 5, Players = new List<string>(5), Roomer = playerId, RoomName = playerId + "的房间",
                RoomId = RoomIDFactory++, RoomState = 0
            };
            roomItem.Players.Add(playerId);
            m_RoomItemDic.Add(roomItem.RoomId,roomItem);
            return roomItem;
        }
        public RoomItem OnJoinRoom(string playerId, long roomId)
        {
            RoomItem roomItem = new RoomItem();
            var state = VerifyPlayerState(playerId);
            if (state != 0)
            {
                roomItem.RoomId = -1;
                return roomItem;
            }

            if (!m_RoomItemDic.TryGetValue(roomId, out roomItem))
            {
                roomItem = new RoomItem {RoomId = -1};
                return roomItem;
            }

            roomItem.Players.Add(playerId);
            Log.Info("当前房间人数 = " + roomItem.Players.Count);
            return roomItem;


        }
        public RoomItem OnExitRoom(string playerId, long roomId)
        {
            RoomItem roomItem = new RoomItem
            {
                RoomId = -1
            };
            var state = VerifyPlayerState(playerId,ref roomItem);

            if (roomItem.RoomId == -1 || state == 0)
            {
                return roomItem;
            }

            if (roomItem.Roomer == playerId)
            {
                CloseRoom(roomItem);
            }
            else
            {
                roomItem.Players.Remove(playerId);
                //todo 通知房间内其他玩家

            }


            return roomItem;

        }
        //todo 关闭房间
        private void CloseRoom(RoomItem roomItem)
        {
            //通知战斗系统关闭战斗

            //通知战斗内玩家结束战斗

            //清空房间，释放各系统资源

        }

        /// <summary>
        /// 0:自由人
        /// 1:非自由人
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private int VerifyPlayerState(string playerId)
        {
            int state = 0;
            foreach (var roomItem in m_RoomItemDic)
            {
                if (roomItem.Value.Players.Contains(playerId))
                    state = 1;
            }
            return state;
        }
        /// <summary>
        /// 0:自由人
        /// 1:非自由人
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private int VerifyPlayerState(string playerId,ref RoomItem item)
        {
            int state = 0;
            foreach (var roomIdAndItem in m_RoomItemDic)
            {
                if (roomIdAndItem.Value.Players.Contains(playerId))
                {
                    item = roomIdAndItem.Value;
                    state = 1;
                }
                    
            }
            return state;
        }

        public RoomItem GetRoomItem(long roomId)
        {
            return m_RoomItemDic[roomId];
        }

        public List<RoomItem> GetRoomList()
        {
            return m_RoomItemDic.Values.ToList();
        }


        private Dictionary<long, RoomItem> m_RoomItemDic = new Dictionary<long, RoomItem>();

        private static long RoomIDFactory = 0;


        
    }




    public class RoomItem
    {
        public long RoomId;

        public string Roomer;

        //---以下变量可以修改
        public string RoomName;

        public int Capacity;

        /// <summary>
        /// 0 开放 1 游戏中 
        /// </summary>
        public int RoomState;

        /// <summary>
        /// 可获取变量
        /// </summary>
        public List<string> Players = new List<string>();



    }
}
