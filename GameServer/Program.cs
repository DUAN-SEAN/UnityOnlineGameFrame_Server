using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.Configure;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer gameServer = new GameServer();
            if (!gameServer.Initialize<SampleGameServerGlobalConfig, SampleGameServerPlayerContext>
                (@"GameServerConfig.config", typeof(SampleGameServerPlayerContext), new ProtobufPacker(), "TankGameServer"))
            {
                Log.Error("初始化服务器错误");
            }
            Log.Trace("服务器初始化成功！！！");
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "close-a")
                {

                }
                Log.Debug(cmd);
            }
        }
    }

   
}
