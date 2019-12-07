using Crazy.NetSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crazy.Common;
namespace Crazy.ServerBase
{
    /// <summary>
    /// 用来保证执行顺序的队列接口，需要线程安全的保证
    /// </summary>
    public interface IContextAsyncActionSequenceQueue
    {
        ContextAsyncAction FirstInActionSeqQueue();
        void EnqueueActionSeqQueue(ContextAsyncAction action);
        bool TryDequeueActionSeqQueue(out ContextAsyncAction action);
    }

    /// <summary>
    /// 用来支持异步action的ILocalMessageClient接口扩展，增加有效性检查和ref检查机制
    /// </summary>
    public interface IAsyncLocalMessageClient : ILocalMessageClient
    {

        /// <summary>
        /// 当前现场是否有效，对于无效ctx不能发送msg
        /// </summary>
        /// <returns></returns>
        bool IsAvaliable();
        /// <summary>
        /// 当有一个新的和当前现场关联的action开始执行是调用
        /// </summary>
        void AddRef4AsyncAction();
        /// <summary>
        /// 获取被action引用的次数
        /// </summary>
        /// <returns></returns>
        int GetRef4AsyncAction();
        /// <summary>
        /// 当一个action执行完毕时调用
        /// </summary>
        void RemoveRef4AsyncAction();
    }

    /// <summary>
    /// 现场异步处理封装，当一个playerctx需要post一个耗时处理,又不想阻塞当前现场，并通过localmsg感知结果,请用这个类
    /// </summary>
    public class ContextAsyncAction : ILocalMessage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="onResultLocalMsgClient">相关联的处理现场</param>
        /// <param name="sequenceQueue">用来保证执行顺序的队列</param>
        /// <param name="needResult">是否需要返回，如果为true，playerCtx会在处理完成之后作为localmsg收到当前对象，并自动调用OnResult回调</param>
        /// <param name="needInSeq">是否需要保证action的执行顺序</param>
        public ContextAsyncAction(IAsyncLocalMessageClient onResultLocalMsgClient = null, IContextAsyncActionSequenceQueue sequenceQueue = null, bool needResult = false, bool needInSeq = false)
        {
            m_onResultLocalMsgClient = onResultLocalMsgClient;
            m_sequenceQueue = sequenceQueue;
            m_needResult = needResult;
            m_needInSeq = needInSeq;
            // 当需要保证顺序的时候必须有结果返回
            if (needInSeq)
            {
                m_needResult = true;
            }

            // 当需要返回的时候playerctx不能为空
            if (m_needResult && m_onResultLocalMsgClient == null)
            {
                throw new Exception("_needResult && _localMsgClient == null");
            }

            // 当需要保持执行顺序的时候队列不能为空
            if (m_needInSeq && sequenceQueue == null)
            {
                throw new Exception("_needInSeq && sequenceQueue == null");
            }
        }

        /// <summary>
        /// 实现ILocalMessage接口
        /// </summary>
        public int MessageId
        {
            get { return ServerBaseLocalMesssageIDDef.LocalMsgAsyncActionResult; }
        }

        /// <summary>
        /// 启动action
        /// </summary>
        public void Start()
        {
            if (m_needInSeq)
            {
                // 入队的状态检查  返回原始值   比较m_state 是否等于 0  等于0 则设置为1
                if (Interlocked.CompareExchange(ref m_state, 1, 0) != 0)
                {
                    return;
                }

                // 首先将自己压入队列
                m_sequenceQueue.EnqueueActionSeqQueue(this);

                // 设置对现场的ref  需要执行结果处理的并且有玩家上下文的
                if (m_needResult && m_onResultLocalMsgClient != null)
                {
                    m_onResultLocalMsgClient.AddRef4AsyncAction();
                }

                // 如果此时队列头上是自己，则执行自己
                if (m_sequenceQueue.FirstInActionSeqQueue() == this)
                {
                    // 设置状态机
                    m_state = 2;

                    //这里不用await
                    var noWarnning = ExecuteInternal(); // 这是个async函数
                }
            }
            else
            {
                // 设置对现场的ref
                if (m_needResult && m_onResultLocalMsgClient != null)
                {
                    m_onResultLocalMsgClient.AddRef4AsyncAction();
                }

                // 设置状态机
                m_state = 2;

                //这里不用await
                var noWarnning = ExecuteInternal(); // 这是个async函数
            }            
        }

        /// <summary>
        /// 内部执行函数
        /// </summary>
        protected async Task ExecuteInternal()
        {
            try
            {
                // 执行真正的操作
                await ExecuteAsync();
            }
            catch(Exception e)
            {
                Log.Error(e);
                m_executingException = e;
            }         
            //执行完成后将消息发送给玩家现场
            if (m_needResult && m_onResultLocalMsgClient != null && m_onResultLocalMsgClient.IsAvaliable())
            {
                m_onResultLocalMsgClient.PostLocalMessage(this);
                
            }
        }

        /// <summary>
        /// 业务逻辑
        /// </summary>
        virtual public Task ExecuteAsync()
        {
            // dongsomthing
            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行_needInSeq相关处理
        /// </summary>
        public void OnResultInternal()
        {
            ContextAsyncAction action = null;

            // 设置状态
            m_state = 3;

            // 设置对现场的ref
            if (m_onResultLocalMsgClient != null)
            {
                m_onResultLocalMsgClient.RemoveRef4AsyncAction();
            }

            if (m_needInSeq)
            {
                if (m_sequenceQueue.FirstInActionSeqQueue() == this)
                {
                    m_sequenceQueue.TryDequeueActionSeqQueue(out action);
                }

                // 执行业务逻辑
                if (m_onResultLocalMsgClient.IsAvaliable()) // 可能由于时序问题到这里的时候现场已经无效
                {
                    OnResult();
                }

                // 触发下一个排序action
                action = m_sequenceQueue.FirstInActionSeqQueue();
                if (action != null)
                {
                    action.m_state = 2;
                    //这里不用await
                    var noWarnning = action.ExecuteInternal(); // 这是个async函数
                }
            }
            else
            {
                OnResult();
            }
        }

        /// <summary>
        /// 业务逻辑
        /// </summary>
        virtual public void OnResult()
        { 
            // process result
        }

  
     

        /// <summary>
        /// 刷新LoggableString的格式
        /// </summary>
        public void RefreshLoggableString()
        {
            m_toLoggableString = null;
        }

        /// <summary>
        /// 刷新StatLogString的格式
        /// </summary>
        public void RefreshStatLogString()
        {
            RefreshLoggableString();
        }

        /// <summary>
        /// 状态， 0 idle, 1 inqueue, 2 runing, 3 resulting
        /// </summary>
        private int m_state = 0;
        protected IAsyncLocalMessageClient m_onResultLocalMsgClient = null;
        protected IContextAsyncActionSequenceQueue m_sequenceQueue = null;   // 用来保证执行顺序的队列
        protected bool m_needResult = false;
        protected bool m_needInSeq = false;

        /// <summary>
        /// 用来保存ToLoggableString的
        /// </summary>
        protected String m_toLoggableString = null;

        /// <summary>
        /// 执行过程中的exception
        /// </summary>
        public Exception m_executingException = null; 
    }
}
