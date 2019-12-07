using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using GameServer.Configure;

namespace TankOLGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TankGameServer gameServer = new TankGameServer();
            if (!gameServer.Initialize<TankGameServerGlobalConfig, TankGameServerPlayerContext>
                (@"GameServerConfig.config", typeof(GameServerPlayerContext), new ProtobufPacker(), "TankGameServer"))
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
