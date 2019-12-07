using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.ServerBase;

namespace TankOLGameServer
{
    class TankGameServer:ServerBase
    {
        public TankGameServer() : base()
        {
            m_instance = this;
        }
        /// <summary>
        /// 静态实例
        /// </summary>
        public new static TankGameServer Instance => (TankGameServer)(ServerBase.Instance);

        public override bool Initialize<GlobalConfigureType, PlayerContextBase>(string globalPath, Type plyaerContextType, IMessagePacker messagePraser, string serverName)
        {
            //初始化程序集
            TypeManager.Instance.Add(DLLType.Common, Assembly.GetAssembly(typeof(TypeManager)));
            TypeManager.Instance.Add(DLLType.ServerBase, Assembly.GetAssembly(typeof(ServerBase)));
            TypeManager.Instance.Add(DLLType.GameServer, Assembly.GetAssembly(typeof(TankGameServer)));

            if (!base.Initialize<GlobalConfigureType, PlayerContextBase>(globalPath, plyaerContextType, messagePraser, serverName))
            {
                return false;
            }
            //获取当前服务器的配置文件
            m_gameServerGlobalConfig = base.m_globalConfigure as GameServer.Configure.TankGameServerGlobalConfig;

            //数据库配置
            var dbConfig = m_gameServerGlobalConfig.DBConfigInfos[0];
            //Log.Info($"ip:{dbConfig.ConnectHost} port:{dbConfig.Port} serviceName:{dbConfig.DataBase} username:{dbConfig.UserName} password:{dbConfig.Password}");

            //GCNotification.GCDone += i =>
            //{
            //    Log.Debug("GC = " + i);

            //};
            Log.Debug("GameServer is running with server GC = " + GCSettings.IsServerGC);
            //MongoDBHelper.CreateDBClient(); //测试
            //mongodb测试

            //MongoDBHelper.Test();


            //初始化功能服务的各个模块系统
            if (!InitializeSystem())
            {
                Log.Info("初始化模块系统失败");
                return false;
            }
            //下面可以写启动逻辑线程 将上述游戏逻辑丢到逻辑线程中处理
            return true;
        }
        /// <summary>
        /// 初始化服务器的各个系统
        /// </summary>
        public override bool InitializeSystem()
        {
            // 匹配系统初始化
            //Activator.CreateInstance();
            //Type type = Type.GetType("BattleSystem");
            //TypeManager.Instance.
            //Log.Info(type.ToString());

            //var gameMatchSystem = new GameMatchSystem();
            //if (!gameMatchSystem.Initialize(m_serverId))
            //{
            //    Log.Error("初始化匹配系统失败");
            //    return false;
            //}
            //m_systemDic.Add(gameMatchSystem.GetType(), gameMatchSystem);
            ////战斗系统初始化
            //var battleSystem = new BattleSystem();
            //if (!battleSystem.Initialize())
            //{
            //    Log.Error("初始化战斗系统失败");
            //    return false;
            //}
            //m_systemDic.Add(battleSystem.GetType(), battleSystem);

            //启动各个系统的Tick功能
            foreach (var item in m_systemDic.Values)
            {
                item.Start();//首先System.Start
                var timeId = TimerManager.SetLoopTimer(100, item.Update);
                //timeId保留了一个取消token 后期需要使用用来关闭系统循环
            }
            return true;
        }

        public T GetSystem<T>() where T : BaseSystem
        {
            m_systemDic.TryGetValue(typeof(T), out BaseSystem t);
            return t as T;
        }



        /// <summary>
        /// 获取当前服务器特定配置数据
        /// </summary>
        public GameServer.Configure.TankGameServerGlobalConfig m_gameServerGlobalConfig { get; private set; }
        /// <summary>
        /// 服务器用于顺序化AsyncAction的队列池，根据每个Context的UserId来分配该Context对应的AsyncAction所属的队列
        /// </summary>


    }
}
