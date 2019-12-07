using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Crazy.ServerBase
{
    /// <summary>
    /// Timer节点
    /// </summary>
    public class TimerNode:IEqualityComparer<TimerNode>
    {
        /// <summary>
        /// IEqualityComparer 的Equals重载
        /// </summary>
        /// <param name="equalsA">比较项A</param>
        /// <param name="equalsB">比较项B</param>
        /// <returns>是否一致，一致为0</returns>
        public Boolean Equals(TimerNode equalsA, TimerNode equalsB)
        {
            return (equalsA.m_id == equalsB.m_id);
        }

        // IEqualityComparer 的GetHashCode重载
        /// <summary>
        /// IEqualityComparer 的GetHashCode重载
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>该对象的哈希值</returns>
        public Int32 GetHashCode(TimerNode obj)
        {
            TimerNode timerNode = (TimerNode)obj;
            return (Int32)timerNode.m_id;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_sessionId">唯一ID</param>
        /// <param name="period">调用时间间隔</param>
        /// <param name="cts">任务控制对象</param>
        /// <param name="timerCallBack">回调函数</param>
        /// <param name="int32Data">回调参数</param>
        /// <param name="int64Data">回调参数</param>
        /// <param name="objData">回调参数</param>
        public TimerNode(Int64 id, Int32 period, CancellationTokenSource cts, TimerManager.OnTimerCallBack timerCallBack, Int32 int32Data, Int64 int64Data, Object objData)
        {
            m_taskState = 1;
            m_id = id;
            m_nextCallTime = 0;
            m_period = period;
            m_timerCallBack = timerCallBack;
            m_cts = cts;
        }

        /// <summary>
        /// 到时调用
        /// </summary>
        public void MakeTimerCall()
        {
            m_timerCallBack(m_int32Data,m_int64Data,m_objData);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        public void CancelTask()
        {
            if (m_task==null || m_cts==null)
                return;
            if (Interlocked.CompareExchange(ref m_taskState, 2, 1) != 1)
                return;
            m_cts.Cancel();
            m_cts.Dispose();
            m_task.Wait();
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public Int64 m_id;

        /// <summary>
        /// 下次调用时间，单位万分之一毫秒
        /// </summary>
        public Int64 m_nextCallTime;

        /// <summary>
        /// 调用时间间隔毫秒为单位
        /// </summary>
        public Int64 m_period;

        /// <summary>
        /// Task实例
        /// </summary>
        public Task m_task;

#pragma warning disable CS0649 // 从未对字段“TimerNode.m_int32Data”赋值，字段将一直保持其默认值 0
        /// <summary>
        /// 回调参数
        /// </summary>
        private Int32 m_int32Data;
#pragma warning restore CS0649 // 从未对字段“TimerNode.m_int32Data”赋值，字段将一直保持其默认值 0
#pragma warning disable CS0649 // 从未对字段“TimerNode.m_int64Data”赋值，字段将一直保持其默认值 0
        private Int64 m_int64Data;
#pragma warning restore CS0649 // 从未对字段“TimerNode.m_int64Data”赋值，字段将一直保持其默认值 0
#pragma warning disable CS0649 // 从未对字段“TimerNode.m_objData”赋值，字段将一直保持其默认值 null
        private Object m_objData;
#pragma warning restore CS0649 // 从未对字段“TimerNode.m_objData”赋值，字段将一直保持其默认值 null

        /// <summary>
        /// 回调函数
        /// </summary>
        private TimerManager.OnTimerCallBack m_timerCallBack;

        /// <summary>
        /// Task引用的CancellationToken
        /// </summary>
        private CancellationTokenSource m_cts;

        /// <summary>
        /// 任务状态 0:idle 1:runing 2:closing
        /// </summary>
        private Int32 m_taskState;
    }

    /// <summary>
    /// Player Timer节点
    /// </summary>
    class PlayerTimerNode : IEqualityComparer<PlayerTimerNode>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">唯一ID</param>
        /// <param name="lastCallTime">上次调用时间</param>
        /// <param name="period">调用时间间隔</param>
        /// <param name="timerCallBack">回调函数</param>
        /// <param name="key">回调参数</param>
        public PlayerTimerNode(Int64 id, Int64 nextCallTime, UInt64 playerSid, Int32 int32Data, Int64 int64Data, Object objData)
        {
            m_id = id;
            m_nextCallTime = nextCallTime;
            m_playerSid = playerSid;
            m_int32Data = int32Data;
            m_int64Data = int64Data;
            m_objData = objData;
        }

        /// <summary>
        /// IEqualityComparer 的Equals重载
        /// </summary>
        /// <param name="equalsA">比较项A</param>
        /// <param name="equalsB">比较项B</param>
        /// <returns>是否一致，一致为0</returns>
        public Boolean Equals(PlayerTimerNode equalsA, PlayerTimerNode equalsB)
        {
            return (equalsA.m_id == equalsB.m_id);
        }

        // IEqualityComparer 的GetHashCode重载
        /// <summary>
        /// IEqualityComparer 的GetHashCode重载
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>该对象的哈希值</returns>
        public Int32 GetHashCode(PlayerTimerNode obj)
        {
            PlayerTimerNode timerNode = (PlayerTimerNode)obj;
            return (Int32)timerNode.m_id;
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public Int64 m_id;

        /// <summary>
        /// Player ID
        /// </summary>
        public UInt64 m_playerSid;

        /// <summary>
        /// 下次调用时间
        /// </summary>
        public Int64 m_nextCallTime;

        /// <summary>
        /// 回调参数
        /// </summary>
        public Int32 m_int32Data;
        public Int64 m_int64Data;
        public Object m_objData;        
    }
}
