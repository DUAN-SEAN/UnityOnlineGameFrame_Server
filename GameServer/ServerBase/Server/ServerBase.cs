
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Crazy.NetSharp;
using Crazy.Common;
using System.Net;

namespace Crazy.ServerBase
{
    public enum ServerType
    {
        Gate,Game,Lobby
    }
    public partial class ServerBase:IServiceEventHandler //提供Service使用事件
    {
        public ServerBase()
        {
            m_instance = this;
        }
        /// <summary>
        /// 服务器初始化 配置文件初始化、协议字典初始化、网络初始化
        /// </summary>
        /// <typeparam name="GlobalConfigureType"></typeparam>
        /// <param name="globalPath"></param>
        /// <param name="plyaerContextType"></param>
        /// <param name="messageDispather"></param>
        /// <param name="opcodeTypeDictionary"></param>
        /// <param name="messagePraser"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public virtual bool Initialize<GlobalConfigureType,PlayerContextBase>(string globalPath,Type plyaerContextType,IMessagePacker messagePraser,string serverName
        )
             where GlobalConfigureType : ServerBaseGlobalConfigure, new()
             where PlayerContextBase:class
        {


            if (!InitlizeLogConfigure())
            {
                Log.Error("初始化日志系统失败");
                return false;
            }

            //默认为protobufPraser
            m_messagePraser = messagePraser;
     
            //初始化配置文件
            if (!InitlizeServerConfigure<GlobalConfigureType>(globalPath,serverName))
            {
                Log.Error("初始化配置文件失败");
                return false;
            }
            //初始化上下文管理器
            if (!InitializePlayerContextManager(plyaerContextType,m_configServer.maxPlayerCtx))
            {
                Log.Error("初始化玩家上下文管理器失败");
                return false;
            }
            // 开启timer管理器用于服务器Tick
            Log.Info("ServerBase::Initialization is starting timer manager");
            TimerManager = new TimerManager(PlayerCtxManager);
            if (TimerManager.Start() < 0)
            {
                Log.Error("ServerBase::Initialization started timer manager failed");
                return false;
            }

            //初始化网络
            if (!InitializeNetWork())
            {
                Log.Error("配置网络出现错误");
                return false;
            }
           
            return true;
        }
        /// <summary>
        /// 初始化网络
        /// </summary>
        private bool InitializeNetWork()
        {
            if (!InitlizeServerProtobuf())
            {
                Log.Error("初始化protobuf消息组件失败");
                return false;
            }
            if (m_configServer == null)
            {
                Log.Error("配置文件未找到当前服务器的配置信息");
                return false;
            }
            
            //生成服务 监听端口
            m_service = new Service();
            if(m_service == null)
            {
                Log.Error("服务启动失败");
                return false;
            }
            m_service.Start(IPAddress.Parse(m_configServer.EndPortIP), m_configServer.EndPortPort, this);
            Log.Info($"服务启动成功！\nIP<{m_configServer.EndPortIP}>:Port<{m_configServer.EndPortPort}>");
            return true;
        }


        /// <summary>
        /// 关闭服务，之后可以选择重启
        /// </summary>
        /// <returns></returns>
        public virtual bool Dispose()
        {
            return true;
        }
        /// <summary>
        /// 由网络层Service触发的事件
        /// 内部将注册玩家上下文
        /// </summary>
        /// <param name="client">网络层Client</param>
        /// <returns></returns>
        public Task<IClientEventHandler> OnConnect(IClient client)
        {
            if (client == null)
            {
                Log.Error("OnConnect, but client is null");
            }

            Log.Debug("Ready one OnConnect");

            // 创建一个PlayerContext对象
            // 注意这里的PlayerConetext和玩家逻辑线程上下文不一样 这里只允许客户端与服务器的上下文注册
            var playerCtx = PlayerCtxManager.AllocPlayerContext() as PlayerContextBase;
            if (playerCtx == null)
            {
                Log.Error("OnConnect player context allocted failed");
                return Task.FromResult<IClientEventHandler>(null);

            }

       
            client.SetSocketRecvBufferSize(m_globalConfigure.Global.Network.SocketInputBufferLen);
            client.SetSocketSendBufferSize(m_globalConfigure.Global.Network.SocketOutputBufferLen);

            // 将client和ctx关联起来
            playerCtx.AttachClient(client);

            // 通知玩家现场连接完成
            playerCtx.OnConnected();

            // 包装一个完成的Task返回 
            return Task.FromResult<IClientEventHandler>(playerCtx);
        }

        public void OnException(Exception ex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 初始化底层的玩家上下文对象管理器。
        /// </summary>
        /// <param name="playerContextType">业务层最终玩家上下文对象类型，必须有无参数的构造函数。</param>
        /// <param name="playerContextMax"></param>
        /// <returns>初始化是否成功。</returns>
        protected virtual bool InitializePlayerContextManager(Type playerContextType, int playerContextMax = int.MaxValue)
        {
            try
            {
                PlayerCtxManager = new PlayerContextManager();
                PlayerCtxManager.Initialize(playerContextType, m_serverId, playerContextMax);
            }
            catch (Exception e)
            {
                Log.Error($"ServerBase::InitializePlayerContextManager{e}");
                return false;
            }

            return true;
        }

        //配置文件读取 服务器全局配置、游戏配置、NLog配置、mongoDB配置

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="TGlobalConfigClass"></typeparam>
        /// <param name="globalConfPath"></param>
        /// <param name="serverDNName"></param>
        /// <returns></returns>
        public virtual bool InitlizeServerConfigure<TGlobalConfigClass>(string globalConfPath,
            string serverDNName)
            where TGlobalConfigClass :ServerBaseGlobalConfigure, new()
        {
            //1:读取本地配置文件
            m_globalConfigure =  Util.Deserialize<TGlobalConfigClass>(globalConfPath);
            if(m_globalConfigure == null)
            {

                return false;
            }
            //2:通过查找serverName这唯一的服务器名称查找对应的服务配置并初始化m_server(Global.Server)
            m_configServer = m_globalConfigure.Global.Servers[0];

            m_serverId = m_configServer.Id;

            return true;

        }
        protected virtual bool InitlizeLogConfigure()
        {
            Log.Info("初始化日志成功");
            return true;
        }
        //protected virtual bool InitlizeObjectFactory()
        //{
        //    m_objectPool = new ObjectPool();
        //    if (m_objectPool == null) return false;
        //    if (!MessageFactory.Adapting(m_objectPool)) return false;
        //    return true;

        //}
        /// <summary>
        /// 用来初始化两个组件
        /// </summary>
        /// <returns></returns>
        protected virtual bool InitlizeServerProtobuf()
        {
            MessageDispather = new MessageDispather();
            OpcodeTypeDic = new OpcodeTypeDictionary();

            if (!OpcodeTypeDic.Init()||!MessageDispather.Init())
            {
                Log.Error("InitlizeServerProtobuf FAIL!!!");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 初始化子系统
        /// Server必须重写此方法
        /// </summary>
        /// <returns></returns>
        public virtual bool InitializeSystem()
        {
            return true;
        }

        /// <summary>
        /// 向功能系统发送本地消息
        /// </summary>
        /// <typeparam name="System"></typeparam>
        /// <param name="msg"></param>
        public void PostMessageToSystem<System>(ILocalMessage msg) where System : BaseSystem
        {
            BaseSystem baseSystem = m_SystemDic[typeof(System)];
            if (baseSystem == null)
            {
                Log.Fatal("TypeSystem::" + typeof(System) + "不存在，消息Message::" + msg.GetType() + "发送失败");
                return;
            }
            baseSystem.PostLocalMessage(msg);
        }
        public void Update()
        {
            var systems = m_SystemDic.Values;
            foreach (var system in systems)
            {
                system.Update();
            }
        }
        protected readonly Dictionary<Type, BaseSystem> m_SystemDic = new Dictionary<Type, BaseSystem>();



        //服务器解包和封包机制 采取protobuf

        private IMessagePacker m_messagePraser;
       

        /// <summary>
        /// 提供网络服务
        /// </summary>
        private Service m_service;
        
        /// <summary>
        /// 配置文件
        /// </summary>
        protected ServerBaseGlobalConfigure m_globalConfigure;
        /// <summary>
        /// 服务器Id
        /// </summary>
        protected Int32 m_serverId;
#pragma warning disable CS0169 // 从不使用字段“ServerBase.m_serverName”
        /// <summary>
        /// 服务器名称
        /// </summary>
        private string m_serverName;
#pragma warning restore CS0169 // 从不使用字段“ServerBase.m_serverName”
        /// <summary>
        ///当前服务器的配置
        /// </summary>
        protected Crazy.Common.Server m_configServer;
        /// <summary>
        /// 消息分发 所有走网络的消息都通过这个进行分发
        /// </summary>
        private MessageDispather MessageDispather;
        /// <summary>
        /// 协议字典
        /// </summary>
        public OpcodeTypeDictionary OpcodeTypeDic;
        /// <summary>
        /// 获取当前服务器的玩家上下文管理对象。
        /// </summary>
        public PlayerContextManager PlayerCtxManager { get; private set; }
        /// <summary>
        /// 获取当前服务器的Timer管理
        /// </summary>
        public TimerManager TimerManager { get; protected set; }
        /// <summary>
        /// 获取关联ServerBase类的句柄。
        /// 继承类可以覆盖来获得更具体的类型。
        /// </summary>
        protected static ServerBase m_instance;
        /// <summary>
        /// 对象池
        /// </summary>
        protected ObjectPool m_objectPool;

        public static ServerBase Instance
        {
            get { return m_instance; }
        }
    }
}
