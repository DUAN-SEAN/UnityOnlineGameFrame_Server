using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    public struct MessageInfo
    {
        public ushort Opcode;
        public IMessage Message;
        public bool flag;
    }
}
