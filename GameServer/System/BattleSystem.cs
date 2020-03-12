using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Crazy.ServerBase;

namespace GameServer.System
{
    /// <summary>
    /// 战斗系统，将来分布式作为战斗服务器的必备部分
    /// </summary>
    public class BattleSystem:BaseSystem
    {
        public override void Start()
        {
            base.Start();
            m_BattleEntityDic = new Dictionary<long, BattleEntity>();
        }

        public override void Update(int data1 = 0, long data2 = 0, object data3 = null)
        {
            base.Update(data1, data2, data3);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override Task OnMessage(ILocalMessage msg)
        {

            switch (msg.MessageId)
            {
                case BattleSystemLocalMessageIDDef.SyncMsgBattleSystemMessageID:
                    OnSyncPlayerEntity((msg as SyncBattleMsgSystemMessage)?.PlayerSyncEntityMessage);
                    break;
                case ServerBaseLocalMesssageIDDef.NetClientDisConnectMessageDef:
                    OnPlayerDisConnect((msg as NetClientDisConnectMessage).PlayerContextName);
                    break;
                default:
                    break;
            }

            return base.OnMessage(msg);


        }
        /// <summary>
        /// 当玩家断开连接事件发生时
        /// </summary>
        /// <param name="playerId"></param>
        private void OnPlayerDisConnect(string playerId)
        {
            long roomFlag = -1L;
            foreach (var IDandBattleEntity in m_BattleEntityDic)
            {
                long roomId = IDandBattleEntity.Key;
                var battleEntity = IDandBattleEntity.Value;
                if (battleEntity.Roomer == playerId)
                {
                    roomFlag = roomId;
                    //todo 这里要直接通告所有玩家断开连接,目前逻辑暂时先不写
                }


                if (battleEntity.Players.Contains(playerId))
                {
                    roomFlag = roomId;
                    battleEntity.Players.Remove(playerId);
                    BroadcastBattleEntity(
                        new B2C_MissingPlayerEntityBattleMessage {PlayerId = playerId, RoomId = roomId },
                        roomId);
                }
            }

            if (m_BattleEntityDic.TryGetValue(roomFlag, out var b))
            {
                if (b == null || b.Players.Count <= 0)
                {
                    m_BattleEntityDic.Remove(roomFlag);
                }
            }
            
        }

        /// <summary>
        /// 处理玩家同步过来的玩家实体位置
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncPlayerEntity(ISyncBattleMessage msg)
        {
            if (msg == null) return;
            if (msg.GetType() == typeof(SyncStartLevelBattleMessage))
            {
                Log.Info("ISyncBattleMessage::Type = " + msg.GetType());
                Log.Msg(msg);
            }

            BroadcastBattleEntity(msg,msg.RoomId);

        }
        public bool EnterClubBattle(string playerId, long roomId)
        {
            if (!m_BattleEntityDic.TryGetValue(roomId, out var battleEntity))
            {
                battleEntity = new BattleEntity();

                m_BattleEntityDic.Add(roomId,battleEntity);
            }
            battleEntity.Players.Add(playerId);
            return true;

        }

        public void BroadcastBattleEntity(IMessage msg,long roomId)
        {
            if (!m_BattleEntityDic.TryGetValue(roomId, out var battleEntity))
            {
                Log.Info("Warning::BroadcastBattleEntity not find battleEntity Id = "+roomId);
                return;
            }
            GameServer.Instance.PlayerCtxManager.BroadcastLocalMessagebyPlayerId(new SystemSendNetMessage { Message = msg }, battleEntity.Players);
            

        }
        public Dictionary<long, BattleEntity> m_BattleEntityDic;

    }

    public class BattleEntity
    {


        public string Roomer;

        public List<string> Players = new List<string>();
    }

}
