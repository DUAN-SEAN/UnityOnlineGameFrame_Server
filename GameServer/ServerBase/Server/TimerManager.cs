using Crazy.Common;
using Crazy.NetSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Crazy.ServerBase
{
    /// <summary>
    /// playerCtx timerPair 消息
    /// </summary>
    public class PlayerTimerMessage : ILocalMessage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="messageID">消息ID</param>
        /// <param name="objData">所带参数</param>
        public PlayerTimerMessage(Int32 int32Data, Int64 int64Data, Object objData)
        {
            m_objData = objData;
            m_int32Data = int32Data;
            m_int64Data = int64Data;
        }

        // ILocalMessage 接口实现
        public Int32 MessageId
        {
            get { return ServerBaseLocalMesssageIDDef.LocalMsgPlayCtxTimer; }
        }

        /// <summary>
        /// 所带参数
        /// </summary>
        public Object m_objData;

        /// <summary>
        /// 所带参数
        /// </summary>
        public Int32 m_int32Data;

        /// <summary>
        /// 所带参数
        /// </summary>
        public Int64 m_int64Data;        
    }

    /// <summary>
    /// Timer管理类
    /// </summary>
    public class TimerManager
    {
        /// <summary>
        /// 回调函数定义
        /// </summary>
        /// <param name="int32Data">回调参数</param>
        /// <param name="int64Data">回调参数</param>
        /// <param name="objData">回调参数</param>
        public delegate void OnTimerCallBack(Int32 int32Data, Int64 int64Data, Object objData);

        /// <summary>
        /// 构造
        /// </summary>
        public TimerManager(PlayerContextManager playerCtxManager)
        {
            m_state = 0;
            m_playerCtxTimersCancelTokenSource = new CancellationTokenSource();
            m_loopTimers = new ConcurrentDictionary<Int64, TimerNode>();
            m_playerCtxTimers = new ConcurrentDictionary<Int64, PlayerTimerNode>();

            // 保存ctx manger
            m_playerCtxManager = playerCtxManager;
        }

        /// <summary>
        /// 设置循环Timer
        /// </summary>
        /// <param name="period">循环间隔时间，单位：毫秒</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="key">回调参数</param>
        /// <returns>该循环Timer唯一ID Int64.MinValue表示失败</returns>
        public Int64 SetLoopTimer(Int32 period, OnTimerCallBack callBack, Int32 int32Data = 0, Int64 int64Data = 0, Object objData = null)
        {
            if (period < 100 || m_state != 1)
                return Int64.MinValue;

            // 获取该循环Timer唯一ID
            Int64 timerId = Interlocked.Increment(ref m_timerIdGenerator);

            CancellationTokenSource cts = new CancellationTokenSource();

            // 创建该循环Timer
            TimerNode timerNode = new TimerNode(timerId, period, cts, callBack, int32Data, int64Data, objData);

            // 创建该Timer循环任务
            timerNode.m_task = TimerWorkerProc(cts.Token, timerNode);

            // 尝试添加
            Boolean bRet = m_loopTimers.TryAdd(timerId, timerNode);
            if(!bRet)
            {
                throw new Exception("fail to TryAdd timerPair in _loopTimers timerid = " + timerId.ToString());
            }
        
            // _log.InfoFormat("SetLoopTimer {0} {1} is start", timerId, period.ToString());
            return timerId;
        }

        /// <summary>
        /// 移除一个循环Timer
        /// </summary>
        /// <param name="id">该循环Timer唯一ID</param>
        /// <returns>成功：该循环Timer唯一ID，失败：负值</returns>
        public Boolean UnsetLoopTimer(Int64 timerId)
        {
            TimerNode timerNode = null;

            // 先从容器内移除
            Boolean ret = m_loopTimers.TryRemove(timerId, out timerNode);

            // 对timer执行cancel操作
            if (null != timerNode)
            {
                timerNode.CancelTask();
            }

            return ret;
        }

        /// <summary>
        /// 设置单一Timer
        /// </summary>
        /// <param name="playerSid">palyer ID</param>
        /// <param name="period">timerPair limit单位：毫秒</param>
        /// <param name="messageID">message id</param>
        /// <param name="objData">message data</param>
        public Int64 SetPlayerContextTimerOneShoot(UInt64 playerSid, Int32 period, Int32 int32Data, Int64 int64Data, Object objData)
        {
            if (period < 100 )
                return Int64.MinValue;
            if (1 != m_state)
                return Int64.MinValue;

            // 设置第一次调用时间
            Int64 nextCallTime = DateTime.Now.Ticks + (Int64)period * 10000;

            // 获取该循环Timer唯一ID
            Int64 timerId = Interlocked.Increment(ref m_timerIdGenerator);

            // 创建该Player Timer
            PlayerTimerNode playerTimerNode = new PlayerTimerNode(timerId, nextCallTime, playerSid, int32Data, int64Data, objData);

            // 尝试添加
            Boolean bRet = m_playerCtxTimers.TryAdd(timerId, playerTimerNode);
            if (!bRet)
            {
                throw new Exception("fail to TryAdd timerPair in _playerCtxTimers timerid = " + timerId.ToString());
            }

            return timerId;
        }

        // 这个函数用来移除指定的playercontexttimer
        public Boolean UnsetPlayerContextTimer(Int64 timerId)
        {
            PlayerTimerNode timerNode;
            Boolean ret = m_playerCtxTimers.TryRemove(timerId, out timerNode);
            return ret;
        }

        /// <summary>
        /// 开启Timer
        /// </summary>
        /// <returns>0:成功</returns>
        public Int32 Start()
        {
            Log.Info("TimerManager::Start");
            Log.Info("TimerManager::Start is checking state flag");
            if (0 != Interlocked.CompareExchange(ref m_state, 1, 0))
            {
                Log.Error("TimerManager::Start checked state flag failed");
                return -1;
            }
            m_playerCtxTimerTask = PlayerCtxTimerWorkerProc(m_playerCtxTimersCancelTokenSource.Token);
            Log.Info("TimerManager::Start end of being called successful");
            return 0;
        }

        /// <summary>
        /// 结束Timer
        /// </summary>
        public Boolean Stop()
        {
            // 检查标志
            if (1 != Interlocked.CompareExchange(ref m_state, 2, 1))
                return false;

            // 清空player timer容器
            // 先清空player timer任务
            if (null != m_playerCtxTimerTask)
            {
                m_playerCtxTimersCancelTokenSource.Cancel();
                m_playerCtxTimersCancelTokenSource.Dispose();
                m_playerCtxTimerTask.Wait();
            }
            // 再清空player timer容器
            m_playerCtxTimers.Clear();

            // 清空循环timer容器
            // 先清空循环timer任务
            foreach (KeyValuePair<Int64, TimerNode> value in m_loopTimers)
            {
                if (value.Value==null)
                    continue;
                value.Value.CancelTask();
            }
            // 再清空循环timer容器
            m_loopTimers.Clear();

            return true;
        }

        

        /// <summary>
        /// 单个TimerNode的循环Timer工作
        /// </summary>
        /// <param name="cancelToken">控制取消对象</param>
        /// <param name="timerNode">单个TimerNode对象</param>
        /// <returns></returns>
        private async Task TimerWorkerProc(CancellationToken cancelToken, TimerNode timerNode)
        {
            // 检查工作状态
            if (1 != m_state)
                return;

            try
            {
                Int64 currentTimeTick = 0;
                Int64 delayMillSeconds = 0;

                // 没有收到取消请求时
                while (!cancelToken.IsCancellationRequested)
                {
                    // 获取处理开始时间毫微秒
                    currentTimeTick = DateTime.Now.Ticks;

                    // 第一次设置时间
                    if (timerNode.m_nextCallTime == 0)
                    {
                        timerNode.m_nextCallTime = currentTimeTick + timerNode.m_period * 10000;//*10000 转换成tick单位
                    }

                    if (currentTimeTick >= timerNode.m_nextCallTime)
                    {
                        try
                        {
                            timerNode.MakeTimerCall();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("TimerManager::TimerWorkerProc catch Exception0: "+ ex.ToString());
                        }
                        timerNode.m_nextCallTime = currentTimeTick + timerNode.m_period * 10000;
                    }

                    // 获取处理结束时间毫秒
                    delayMillSeconds = timerNode.m_nextCallTime - DateTime.Now.Ticks;
                    if (delayMillSeconds > 10000000) // 大于1秒休息一秒
                    {
                        await Task.Delay(1000);
                    }
                    else if (delayMillSeconds > 10000)// 否则休息指定时间
                    {
                        await Task.Delay((int)delayMillSeconds / 10000);
                    }
                }
                Log.Info("TimerManager::TimerWorkerProc stop timerid="+timerNode.m_id);
            }
            catch (Exception ex)
            {
                Log.Error("TimerManager::TimerWorkerProc catch Exception1: "+ex.ToString());
            }
        }

        /// <summary>
        /// Palyer Timer工作
        /// </summary>
        /// <param name="cancelToken">控制取消对象</param>
        /// <returns></returns>
        private async Task PlayerCtxTimerWorkerProc(CancellationToken cancelToken)
        {
            // 检查工作状态
            if (1 != m_state)
                return;
            try
            {
                //Int64 currentTimeTick = 0;

                // 没有收到取消请求时
                while (!cancelToken.IsCancellationRequested)
                {
                    //// 获取开始时间毫秒
                    //currentTimeTick = DateTime.Now.Ticks;

                    //// 用当前时间秒查询循环容器
                    //IEnumerable<KeyValuePair<Int64, PlayerTimerNode>> query =
                    //    m_playerCtxTimers.Where(timer => timer.Value.m_nextCallTime <= currentTimeTick);

                    // 有满足条件的作timer call处理以及删除
                    foreach (KeyValuePair<Int64, PlayerTimerNode> timerPair in GetTriggerableTimerNodeList())
                    {
                        // 先删除
                        PlayerTimerNode playerTimer = null;
                        m_playerCtxTimers.TryRemove(timerPair.Key, out playerTimer);

                        if (playerTimer == null)
                            continue;

                        // 查找目标playerctx
                        IManagedContext playerCtx = m_playerCtxManager.FindPlayerContextBySid(playerTimer.m_playerSid);
                        if (playerCtx == null)
                            continue;

                        // 发送msg
                        PlayerTimerMessage message = new PlayerTimerMessage(playerTimer.m_int32Data, playerTimer.m_int64Data, playerTimer.m_objData);
                        playerCtx.PostLocalMessage(message);
                    }

                    // 每500毫秒轮询一次
                    await Task.Delay(500);
                }
                Log.Debug("TimerManager::PlayerCtxTimerWorkerProc stop");
            }
            catch (Exception ex)
            {
                Log.Debug("TimerManager::PlayerCtxTimerWorkerProc catch Execption: "+ ex.ToString());
            }
        }

        /// <summary>
        /// 获取当前需要触发的TimerNode
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<Int64, PlayerTimerNode>> GetTriggerableTimerNodeList()
        {
            // 获取时间毫秒
            var currentTimeTick = DateTime.Now.Ticks;
            // 判断是否需要触发
            foreach (var it in m_playerCtxTimers)
            {
                if (it.Value.m_nextCallTime <= currentTimeTick)
                {
                    yield return it;
                }
            }
        }

        /// <summary>
        /// Timer状态 0：idle 1：工作 2：结束
        /// </summary>
        private Int32 m_state = 0;
        public Int32 State
        {
            get { return m_state; }
        }

        /// <summary>
        /// 当前分配的TimerID
        /// </summary>
        private Int64 m_timerIdGenerator = 0;

        /// <summary>
        /// 无限循环的Timer容器
        /// </summary>
        private ConcurrentDictionary<Int64, TimerNode> m_loopTimers;

        /// <summary>
        /// 玩家的Timer容器
        /// </summary>
        private ConcurrentDictionary<Int64, PlayerTimerNode> m_playerCtxTimers;

        /// <summary>
        /// Task引用的CancellationToken
        /// </summary>
        private CancellationTokenSource m_playerCtxTimersCancelTokenSource;

        /// <summary>
        /// 用来进行playerTimer的ctxManager
        /// </summary>
        private PlayerContextManager m_playerCtxManager;

        /// <summary>
        /// Task实例
        /// </summary>
        private Task m_playerCtxTimerTask;

 
    }
}
