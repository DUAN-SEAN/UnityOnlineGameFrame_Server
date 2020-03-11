using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;

namespace GameServer
{
    public class SyncBattleMsgSystemMessage : ILocalMessage
    {
        public int MessageId { get=> BattleSystemLocalMessageIDDef.SyncMsgBattleSystemMessageID; }

        public ISyncBattleMessage PlayerSyncEntityMessage;
        

    }




   
}
