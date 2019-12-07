using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.NetSharp;
using Crazy.Common;
namespace Crazy.ServerBase
{
    /// <summary>
    /// 和PlayerContextManager配合使用的可管理现场接口
    /// </summary>
    public interface IManagedContext : IAsyncLocalMessageClient
    {
        UInt64 ContextId { get; set; }
        String ContextStringName { get; }
    }
    /// <summary>
    /// 玩家现场管理类 功能：
    /// 1、分配玩家现场
    /// 2、进行全局广播
    /// 3、回收玩家现场
    /// </summary>
    public class PlayerContextManager
    {

        public void Initialize(Type playerContextType, Int32 serverId, Int32 sizeMax)
        {
            // 保存最终PlayerContext的类型
            m_contextType = playerContextType;

            // 保存最大现场个数 
            m_sizeMax = sizeMax;

            /// 测试给定的类型是否符合规范
            IManagedContext newCtx = Activator.CreateInstance(m_contextType) as IManagedContext;
            if (newCtx == null)
                throw new ArgumentException("PlayerContextManager::Initialize The type of playerContextType is not inherit from IManagedPlayerContext。");

            // 构造sessionidFactory
            m_sessionIdFactory = new SessionIdFactory(serverId);

            /// 创建字典对象
            m_playerSid2CtxDic = new ConcurrentDictionary<UInt64, IManagedContext>();
            m_playerCtxName2CtxDic = new ConcurrentDictionary<String, IManagedContext>();
        }

        /// <summary>
        /// 分配一个玩家上下文对象并管理到底层中。
        /// </summary>
        /// <returns>玩家上下文对象的实例。</returns>
        public IManagedContext AllocPlayerContext()
        {
            // 超过大小不能分配
            if (GetSize() >= SizeMax)
            {
                return null;
            }

            /// 创建一个新现场对象
            /// 之后可以建立对象池 而不需要每次都实例化
            IManagedContext newCtx = Activator.CreateInstance(m_contextType) as IManagedContext;

            /// 生成新的ContexId并绑定到新的Context对象上
            /// 并且管理该新的Context对象到底层字典中
            UInt64 sessionId = m_sessionIdFactory.AllocateSessionId();
            newCtx.ContextId = sessionId;
            if (!m_playerSid2CtxDic.TryAdd(sessionId, newCtx))
            {
                throw new Exception("fail to add ctx in m_playerSid2CtxDic");
            }

            return newCtx;
        }

        /// <summary>
        /// 分配一个玩家上下文对象并管理到底层中。
        /// </summary>
        /// <returns>玩家上下文对象的实例。</returns>
        public IManagedContext AllocPlayerContext(UInt64 sessionId)
        {
            // 超过大小不能分配
            if (GetSize() >= SizeMax)
            {
                return null;
            }

            /// 创建一个新现场对象
            IManagedContext newCtx = Activator.CreateInstance(m_contextType) as IManagedContext;

            /// 生成新的ContexId并绑定到新的Context对象上
            /// 并且管理该新的Context对象到底层字典中
            newCtx.ContextId = sessionId;
            if (!m_playerSid2CtxDic.TryAdd(sessionId, newCtx))
            {
                throw new Exception("fail to add ctx in m_playerSid2CtxDic");
            }

            return newCtx;
        }

        /// <summary>
        /// 解除底层对指定Context的管理。
        /// </summary>
        /// <param name="context">需要解除的管理对象。</param>
        public void FreePlayerContext(IManagedContext context)
        {
            IManagedContext ctx;

            /// 解除Id关联字典 PS 由于使用了线程安全的字典所以不用担心
            m_playerSid2CtxDic.TryRemove(context.ContextId, out ctx);
            /// 解除String关联字典
            if (context.ContextStringName != null)
            {
                IManagedContext nameCtx = null;
                m_playerCtxName2CtxDic.TryGetValue(context.ContextStringName, out nameCtx);

                if (nameCtx != null && ctx != null && nameCtx.ContextId == ctx.ContextId)
                {
                    m_playerCtxName2CtxDic.TryRemove(context.ContextStringName, out nameCtx);
                }
            }
        }

        /// <summary>
        /// 获取当前维护的对象数目。
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return m_playerSid2CtxDic.Count;
        }

        /// <summary>
        /// 注册String与对象关联，对象的Name属性必须要设定并且由用户保证唯一。
        /// </summary>
        /// <param name="playerContext">要关联的对象，其属性Name作为键值。</param>
        /// <returns>注册是否成功。</returns>
        public bool RegisterPlayerContextByString(String stringKey, IManagedContext playerContext)
        {
            if (!m_playerCtxName2CtxDic.TryAdd(stringKey, playerContext))
            {
                return false;
            }
            return true;

            /// 不考虑Id映射字典中是否存在，该对象在创建时已经自动加入了。
        }

        /// <summary>
        /// 注销String与对象关联，但会保留其ID的关联。
        /// </summary>
        /// <param name="stringKey">要注销的对象名称。</param>
        /// <returns>注销是否成功。</returns>
        public bool UnregisterPlayerContextByString(String stringKey)
        {
            IManagedContext ctx;
            if (!m_playerCtxName2CtxDic.TryRemove(stringKey, out ctx))
                return false;
            return true;
        }

        /// <summary>
        /// 根据对象名称查找对象。
        /// </summary>
        /// <param name="stringKey">指定对象的名称。</param>
        /// <returns>查找结果对象。</returns>
        public IManagedContext FindPlayerContextByString(String stringKey)
        {
            IManagedContext player;
            m_playerCtxName2CtxDic.TryGetValue(stringKey, out player);
            return player;
        }

        /// <summary>
        /// 根据对象ID查找对象
        /// </summary>
        /// <param name="id">指定对象的ID</param>
        /// <returns>查找结果对象</returns>
        public IManagedContext FindPlayerContextBySid(UInt64 sid)
        {
            IManagedContext ctx;
            if (m_playerSid2CtxDic.TryGetValue(sid, out ctx))
            {
                return ctx;
            }
            return null;
        }

        public T FindPlayerContextBySid<T>(UInt64 sid)
            where T : class, IManagedContext
        {
            return FindPlayerContextBySid(sid) as T;
        }

        /// <summary>
        /// 获取注册成功的在线玩家人数
        /// </summary>
        /// <returns></returns>
        public Int32 GetRegisteredPlayerCount()
        {
            return m_playerCtxName2CtxDic.Count;
        }

        /// <summary>
        /// 获得当前所有在线玩家的列表数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<UInt64, IManagedContext>> GetRegisteredPlayerList()
        {
            return m_playerSid2CtxDic;
        }

        /// <summary>
        /// 获得当前所有成功登录的玩家列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IManagedContext>> GetRegisteredByStringPlayerList()
        {
            return m_playerCtxName2CtxDic;
        }

        /// <summary>
        /// 获取随机的玩家列表
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="ctxList"></param>
        public void GetRandomPlayerList(int startIndex, int count, List<IManagedContext> ctxList)
        {
            int i = 0;
            foreach (var item in m_playerSid2CtxDic)
            {
                if (i >= startIndex && item.Value.IsAvaliable())
                {
                    ctxList.Add(item.Value);
                }
                i++;

                if (i >= startIndex + count)
                {
                    return;
                }
            }
        }

        public delegate bool BroadcastFilter(IManagedContext playerCtx);

        /// <summary>
        /// 按照ContextIds 多播数据
        /// 适用于contextIds相比于m_playerSid2CtxDic，集合小很多的情况
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="contextIds"></param>
        public void BroadcastLocalMessageByContextIds(ILocalMessage msg, List<UInt64> contextIds)
        {
            IManagedContext context;
            foreach (var id in contextIds)
            {
                if (m_playerSid2CtxDic.TryGetValue(id, out context))
                {
                    context.PostLocalMessage(msg);
                }
            }
        }

        /// <summary>
        /// 广播localmsg
        /// </summary>
        /// <param name="lmsg"></param>
        /// <param name="filter"></param>
        public void BroadcastLocalMessage(ILocalMessage msg, BroadcastFilter filter = null)
        {
            foreach (var item in m_playerSid2CtxDic)
            {
                // 使用过滤器
                if (filter != null && !filter(item.Value))
                {
                    continue;
                }
                item.Value.PostLocalMessage(msg);
            }
        }
        /// <summary>
        /// 向一个玩家发送本地消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="playerId"></param>
        public void SendSingleLocalMessage(ILocalMessage msg,string playerId)
        {
            if (playerId == null || playerId == default) return;
            var playerCtx = FindPlayerContextByString(playerId);
            if (playerCtx != null)
            {
                playerCtx.PostLocalMessage(msg);
            }
                
        }
        /// <summary>
        /// 向多个玩家发送本地消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="playerId"></param>
        public void BroadcastLocalMessagebyPlayerId(ILocalMessage msg, List<string> playerIds)
        {
            if (playerIds == null) return;
            foreach(var id in playerIds)
            {
                var playerCtx = FindPlayerContextByString(id);
                if (playerCtx != null)
                    playerCtx.PostLocalMessage(msg);
            }
        
        }
        /// <summary>
        /// 玩家上下文实际类型。
        /// </summary>
        private Type m_contextType;
        /// <summary>
        /// sessionId工厂
        /// </summary>
        private SessionIdFactory m_sessionIdFactory;
        /// <summary>
        /// 最多现场个数
        /// </summary>
        private Int32 m_sizeMax = 0;
        public Int32 SizeMax { get { return m_sizeMax; } }
        /// <summary>
        /// 对象字典，从ID到对象。
        /// </summary>
        private ConcurrentDictionary<UInt64, IManagedContext> m_playerSid2CtxDic;
        /// <summary>
        /// 对象字典，从String到对象
        /// </summary>
        private ConcurrentDictionary<String, IManagedContext> m_playerCtxName2CtxDic;
    }
}
