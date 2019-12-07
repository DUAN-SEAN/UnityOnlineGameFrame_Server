using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlackJack.Utils
{
    /// <summary>
    /// 异步信号类，用于锁
    /// </summary>
    public class AsyncSemaphore
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialCount">同时允许访问的资源个数</param>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            m_currentCount = initialCount;
        }

        /// <summary>
        /// 可以异步的等待
        /// </summary>
        /// <returns></returns>
        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (m_currentCount > 0)
                {
                    --m_currentCount;
                    return s_completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    m_waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                    toRelease = m_waiters.Dequeue();
                else
                    ++m_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }

        /// <summary>
        /// 成功进入任务返回
        /// </summary>
        private readonly static Task s_completed = Task.CompletedTask;

        /// <summary>
        /// 等待队列
        /// </summary>
        private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();

        /// <summary>
        /// 当前可访问资源个数
        /// </summary>
        private int m_currentCount;
    }
}
