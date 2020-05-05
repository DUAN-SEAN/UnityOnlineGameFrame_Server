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

namespace GateServer
{
   
    public class GateServer:ServerBase
    {
        public GateServer() : base()
        {
            m_instance = this;
        }
        /// <summary>
        /// 静态实例
        /// </summary>
        public new static GateServer Instance => (GateServer)(ServerBase.Instance);

        public override bool Initialize<GlobalConfigureType, PlayerContextBase>(string globalPath, Type plyaerContextType, IMessagePacker messagePraser, string serverName)
        {
            //初始化程序集
            TypeManager.Instance.Add(DLLType.Common, Assembly.GetAssembly(typeof(TypeManager)));
            TypeManager.Instance.Add(DLLType.ServerBase, Assembly.GetAssembly(typeof(ServerBase)));
            TypeManager.Instance.Add(DLLType.GameServer, Assembly.GetAssembly(typeof(GateServer)));

            if (!base.Initialize<GlobalConfigureType, PlayerContextBase>(globalPath, plyaerContextType, messagePraser, serverName))
            {
                return false;
            }
            
            //数据库配置
            var dbConfig = m_gameServerGlobalConfig.DBConfigInfos[0];

            if(!InitializeServerContext())
            {
                return false;
            }

            //下面可以写启动逻辑线程 将上述游戏逻辑丢到逻辑线程中处理
            return true;
        }
        /// <summary>
        /// 与所有内部的服务器进行连接，保存所有的servercontext
        /// </summary>
        /// <returns></returns>
        public virtual bool InitializeServerContext()
        {
            m_serverContextByTypeDic = new Dictionary<ServerType, List<ServerContextBase>>();
            m_serverContextByTypeDic.Add(ServerType.Lobby,new List<ServerContextBase>());
            m_serverContextByTypeDic.Add(ServerType.Game,new List<ServerContextBase>());

            var servers = m_gameServerGlobalConfig.Global.Servers;
            var serverTypes = m_gameServerGlobalConfig.ServerTypes;
            foreach (var serverType in serverTypes)
            {
                var server = servers[serverType.Id - 1];
                IPAddress address = null;
                if (server.EndPortIP == "0.0.0.0")
                {
                    IPAddress.TryParse("127.0.0.1", out  address);
                }
                else
                {
                    IPAddress.TryParse(server.EndPortIP, out  address);
                }
                
                var port = server.EndPortPort;
                Client c = null;
                switch (serverType.Type)
                {
                    case "Lobby":
                        LobbyServerServerContext lobbyServerServerContext = new LobbyServerServerContext(serverType.Id,ServerType.Lobby);
                        lobbyServerServerContext.ContextId = ContextIdFactory++;
                        c = Client.ConnectTo(address, port, lobbyServerServerContext);
                        if(c==null) Log.Info("InitializeServerContext::Client = null ServerId = "+serverType.Id);
                        lobbyServerServerContext.AttachClient(c);
                        lobbyServerServerContext.OnConnected();
                        m_serverContextByIdDic.Add(lobbyServerServerContext.ServerId,lobbyServerServerContext);
                        m_serverContextByTypeDic[ServerType.Lobby].Add(lobbyServerServerContext);
                        // 将client和ctx关联起来
                        
                        break;
                    case "Game":
                        GameServerServerContext gameServerServerContext = new GameServerServerContext(serverType.Id,ServerType.Game);
                        gameServerServerContext.ContextId = ContextIdFactory++;
                        c = Client.ConnectTo(address, port, gameServerServerContext);
                        if (c == null) Log.Info("InitializeServerContext::Client = null ServerId = " + serverType.Id);
                        gameServerServerContext.AttachClient(c);
                        gameServerServerContext.OnConnected();
                        m_serverContextByIdDic.Add(gameServerServerContext.ServerId, gameServerServerContext);
                        m_serverContextByTypeDic[ServerType.Game].Add(gameServerServerContext);
                        break;
                    default:break;
                }

            }
            return true;


        }

      
        /// <summary>
        /// 初始化服务器的各个系统
        /// </summary>
        public override bool InitializeSystem()
        {
           

            //启动各个系统的Tick功能
            foreach (var item in m_SystemDic.Values)
            {
                item.Start();//首先System.Start
                var timeId = TimerManager.SetLoopTimer(100, item.Update);
                //timeId保留了一个取消token 后期需要使用用来关闭系统循环
            }
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


        public async Task<int> CatchServer(ServerType serverType)
        {
            var serverContexts = m_serverContextByTypeDic[serverType];
            List<Task<int>> tasks = new List<Task<int>>();
            int i = 0;
            foreach (var server in serverContexts)
            {
                switch (serverType)
                {
                    case ServerType.Game:
                        GameServerServerContext gameServerServerContext = server as GameServerServerContext;
                        if (gameServerServerContext == null) break;
                        tasks.Add(gameServerServerContext.CatchServer());
                        break;
                    case ServerType.Lobby:
                        LobbyServerServerContext lobbyServerServerContext = server as LobbyServerServerContext;
                        if (lobbyServerServerContext == null) break;
                        tasks.Add(lobbyServerServerContext.CatchServer());
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
                }
               
                
            }

            var taskIndex = await Task.WhenAny(tasks);

            return taskIndex.Result;
        }

        public void SendInnerMessage(int serverId,IMessage message)
        {
            var serverContext = m_serverContextByIdDic[serverId];
            if(serverContext == null)
            {
                Log.Info("SendInnerMessage::serverContext is NULL");
                return;
            }

            serverContext.Send(message);

        }

        public async Task<IResponse> CallInnerMessage(int serverId, IRequest message)
        {
            var serverContext = m_serverContextByIdDic[serverId];
            if (serverContext == null)
            {
                Log.Info("SendInnerMessage::serverContext is NULL");
                return null;
            }

            var response = await serverContext.Call(message);
            return response;
        }
        /// <summary>
        /// 获取当前服务器特定配置数据
        /// </summary>
        public global::GameServer.Configure.SampleGameServerGlobalConfig m_gameServerGlobalConfig { get; private set; }
        
        private Dictionary<int,ServerContextBase> m_serverContextByIdDic = new Dictionary<int, ServerContextBase>();

        private Dictionary<ServerType, List<ServerContextBase>> m_serverContextByTypeDic =
            new Dictionary<ServerType, List<ServerContextBase>>();
        public int ServerId => m_gameServerGlobalConfig.ServerContext.Id;

        private static ulong ContextIdFactory = 1;
    }
}
