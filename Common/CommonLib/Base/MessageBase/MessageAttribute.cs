using System;
using System.Collections.Generic;
using System.Text;

namespace Crazy.Common
{
    public class MessageAttribute : BaseAttribute
    {
        public ushort Opcode { get; }

        public MessageAttribute(ushort opcode)
        {
            this.Opcode = opcode;
        }
    }
}
