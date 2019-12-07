using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Crazy.NetSharp;
using Task = System.Threading.Tasks.Task;

namespace Crazy.ServerBase
{
    /// <summary>
    /// 服务器逻辑系统的基类，由于目前不做分布式系统，所以所有业务服务用System表示一类逻辑
    /// 具体的有匹配系统、物理系统等等
    /// </summary>
    public abstract class BaseSystem : ILocalMessageHandler,ILocalMessageClient
    {


        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Start()
        {
            p_localMessages = new ConcurrentQueue<ILocalMessage>();
        }
        /// <summary>
        /// 驱动更新
        /// 工作线程调用的方法，线程模型为单线程驱动，保证系统的逻辑安全
        /// </summary>
        public virtual void Update()
        {
            ILocalMessage localMessage;
            while(p_localMessages.TryDequeue(out localMessage))
            {
                var nothing = OnMessage(localMessage);
            }
        }
        /// <summary>
        /// 驱动更新
        /// 工作线程调用的方法，线程模型为单线程驱动，保证系统的逻辑安全
        /// </summary>
        public virtual void Update(Int32 data1 = 0,Int64 data2 = 0,object data3 = null)
        {
            
            ILocalMessage localMessage;
            if (p_localMessages.TryDequeue(out localMessage))
            {
                var nothing = OnMessage(localMessage);
            }
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Dispose()
        {
            p_localMessages.Clear();
            p_localMessages = null;
        }
        /// <summary>
        /// 该方法需要被重写
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual Task OnMessage(ILocalMessage msg)
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// 接收传入的本地消息,玩家现场向GameServer发送的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool PostLocalMessage(ILocalMessage msg)
        {
            p_localMessages.Enqueue(msg);
            return true;
        }
        /// <summary>
        /// 向一个玩家发送本地消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="playerId"></param>
        protected void PostLocalMessageToCtx(ILocalMessage msg, string playerId)
        {
            ServerBase.Instance.PlayerCtxManager.SendSingleLocalMessage(msg, playerId);
        }
        /// <summary>
        /// 向多个玩家发送本地消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="playerIds"></param>
        protected void PostLocalMessageToCtx(ILocalMessage msg, List<string> playerIds)
        {
            ServerBase.Instance.PlayerCtxManager.BroadcastLocalMessagebyPlayerId(msg, playerIds);
        }
        protected ConcurrentQueue<ILocalMessage> p_localMessages;


    }

}
