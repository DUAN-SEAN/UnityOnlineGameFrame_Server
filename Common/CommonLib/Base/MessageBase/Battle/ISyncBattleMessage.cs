using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;

namespace Crazy.Common
{

    public interface ISyncBattleMessage : IMessage
    {
        long RoomId { get; set; }
        string PlayerId { get; set; }

        long EntityId { get; set; }
    }
}
