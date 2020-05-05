using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.Configure;

namespace LobbyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LobbyServer server = new LobbyServer();
            if (!server.Initialize<SampleGameServerGlobalConfig, GateServerServerContext>
                (@"LobbyGameServerConfig.config", typeof(GateServerServerContext), new ProtobufPacker(), "TankGameServer"))
            {
                Log.Error("初始化服务器错误");
            }
            Log.Trace("服务器初始化成功！！！");
            while (true)
            {
                //Console.ReadKey();
                LobbyServer.Instance.Update();
                Thread.Sleep(1);
            }
        }
    }
}
