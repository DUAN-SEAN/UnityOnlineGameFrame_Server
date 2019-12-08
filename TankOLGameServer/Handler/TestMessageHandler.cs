using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;

namespace TankOLGameServer.Handler
{
    [MessageHandler]
    public class TestMessageHandler:AMHandler<C2S_TestMessage>
    {
        protected override void Run(ISession playerContext, C2S_TestMessage message)
        {
            Log.Info(message.Data);
        }
    }
}
