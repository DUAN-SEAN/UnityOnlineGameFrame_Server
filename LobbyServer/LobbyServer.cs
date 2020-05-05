using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Crazy.ServerBase;
using GameServer;
using GameServer.System;

namespace LobbyServer
{
    public class LobbyServer : ServerBase
    {
        public LobbyServer() : base()
        {
            m_instance = this;
        }
        /// <summary>
        /// 静态实例
        /// </summary>
        public new static LobbyServer Instance => (LobbyServer)(ServerBase.Instance);

        public override bool Initialize<GlobalConfigureType, PlayerContextBase>(string globalPath, Type plyaerContextType, IMessagePacker messagePraser, string serverName)
        {
            //初始化程序集
            TypeManager.Instance.Add(DLLType.Common, Assembly.GetAssembly(typeof(TypeManager)));
            TypeManager.Instance.Add(DLLType.ServerBase, Assembly.GetAssembly(typeof(ServerBase)));
            TypeManager.Instance.Add(DLLType.GameServer, Assembly.GetAssembly(typeof(LobbyServer)));
            //var types = TypeManager.Instance.GetTypes(typeof(MessageHandlerAttribute));
            //foreach (var type in types)
            //{
            //    Log.Info(type.ToString());
            //}
            if (!base.Initialize<GlobalConfigureType, PlayerContextBase>(globalPath, plyaerContextType, messagePraser, serverName))
            {
                return false;
            }
            
            //数据库配置
            var dbConfig = m_gameServerGlobalConfig.DBConfigInfos[0];

            InitializeSystem();

            //下面可以写启动逻辑线程 将上述游戏逻辑丢到逻辑线程中处理
            return true;
        }
       
        /// <summary>
        /// 初始化服务器的各个系统
        /// </summary>
        public override bool InitializeSystem()
        {
            LobbySystem lobbySystem = new LobbySystem();
            lobbySystem.Start();
            m_SystemDic.Add(lobbySystem.GetType(),lobbySystem);

            ////启动各个系统的Tick功能
            //foreach (var item in m_SystemDic.Values)
            //{
            //    item.Start();//首先System.Start
            //    var timeId = TimerManager.SetLoopTimer(100, item.Update);
            //    //timeId保留了一个取消token 后期需要使用用来关闭系统循环
            //}
            return true;
        }

        public T GetSystem<T>() where T : BaseSystem
        {
            m_SystemDic.TryGetValue(typeof(T), out BaseSystem t);
            return t as T;
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="TGlobalConfigClass"></typeparam>
        /// <param name="globalConfPath"></param>
        /// <param name="serverDNName"></param>
        /// <returns></returns>
        public override bool InitlizeServerConfigure<TGlobalConfigClass>(string globalConfPath,
            string serverDNName)
        {
            //1:读取本地配置文件
            m_globalConfigure = Util.Deserialize<TGlobalConfigClass>(globalConfPath);
            if (m_globalConfigure == null)
            {

                return false;
            }
            //获取当前服务器的配置文件
            m_gameServerGlobalConfig = base.m_globalConfigure as global::GameServer.Configure.SampleGameServerGlobalConfig;


            //2:通过查找serverName这唯一的服务器名称查找对应的服务配置并初始化m_server(Global.Server)
            //m_configServer = m_globalConfigure.Global.Servers[0];
            int indexServer = m_gameServerGlobalConfig.ServerContext.Id - 1;
            m_configServer = m_globalConfigure.Global.Servers[indexServer];

            m_serverId = m_configServer.Id;

            return true;

        }

      


        /// <summary>
        /// 获取当前服务器特定配置数据
        /// </summary>
        public global::GameServer.Configure.SampleGameServerGlobalConfig m_gameServerGlobalConfig { get; private set; }

        /// <summary>
        /// 服务器用于顺序化AsyncAction的队列池，根据每个Context的UserId来分配该Context对应的AsyncAction所属的队列
        /// </summary>
        public int ServerId => m_gameServerGlobalConfig.ServerContext.Id;

      
    }
}
