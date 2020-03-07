using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.NetSharp;

namespace GameServer.System.SystemLocalMessage.Lobby
{
    public class CreateRoomReqLocalMessage : ILocalMessage
    {
        public int MessageId
        {
            get => LobbySystemLocalMessageIDDef.CreateRoomReqLocalMessageID;
        }

        public string PlayerId;
    }


   
}
