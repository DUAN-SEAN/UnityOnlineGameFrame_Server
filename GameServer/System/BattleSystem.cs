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
    public class BattleSystem:BaseSystem
    {
        public override void Start()
        {
            base.Start();
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
                case BattleSystemLocalMessageIDDef.PlayerSyncEntityBattleSystemMessageID:
                    OnSyncPlayerEntity((msg as PlayerSyncEntityBattleSystemMessage)?.PlayerSyncEntityMessage);
                    
                    
                    break;

                default:
                    break;
            }

            return base.OnMessage(msg);


        }
        /// <summary>
        /// 处理玩家同步过来的玩家实体位置
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncPlayerEntity(C2S_PlayerSyncEntityMessage msg)
        {
            if (msg == null) return;

            GameServer.Instance.PlayerCtxManager.BroadcastLocalMessage(new SystemSendNetMessage{Message = msg});

        }

        public override bool PostLocalMessage(ILocalMessage msg)
        {
            return base.PostLocalMessage(msg);
        }
    }
}
