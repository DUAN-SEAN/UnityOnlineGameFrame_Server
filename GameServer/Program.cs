﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            if (!gameServer.Initialize<SampleGameServerGlobalConfig, GateServerServerContext>
                (@"GameServerConfig.config", typeof(GateServerServerContext), new ProtobufPacker(), "GameServer"))
            {
                Log.Error("初始化服务器错误");
            }
            Log.Trace("服务器初始化成功！！！");
            while (true)
            {
                GameServer.Instance.Update();
                Thread.Sleep(1);
            }
        }
    }

   
}
