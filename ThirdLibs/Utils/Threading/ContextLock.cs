using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BlackJack.Utils
{
    //public class ContextLock
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    class WaitableItem
    //    {
    //        public TaskCompletionSource<bool> m_tcs = new TaskCompletionSource<bool>();
    //        public Int64 m_ownerId = 0;
    //        //public Int32 m_threadId = 0;
    //    }

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    public ContextLock(bool reenterable)
    //    {
    //        m_reenterable = reenterable;
    //    }

    //    /// <summary>
    //    /// 等待锁，返回task，调用者必须await或者wait这个task,推荐直接调用AwaitEnter
    //    /// </summary>
    //    /// <param name="owner"></param>
    //    /// <returns></returns>
    //    public Task EnterAsync(Int64 ownerId)
    //    {
    //        //Int32 threadId = Thread.CurrentThread.ManagedThreadId;

    //        if (ownerId == 0)
    //        {
    //            throw new ArgumentException("ownerId can not be 0");
    //        }

    //        lock (this)
    //        {
    //            // 判断ownerid,线程id是否相同, 重入判定
    //            if (m_reenterable && ownerId == m_ownerId) // && threadId == m_threadId)
    //            {
    //                // 两个都相同可以重入, 这里有个隐含的要求一个现场不允许在同一个线程上有两个同时存在的执行现场
    //                m_reenterCount++;
    //                return s_completed;
    //            }

    //            // 如果等于1说明是唯一当前拥有锁的现场
    //            if (Interlocked.Increment(ref m_waiterCount) == 1)
    //            {
    //                m_ownerId = ownerId;
    //                //m_threadId = threadId;
    //                return s_completed;
    //            }
    //            else
    //            {
    //                // 需要等待
    //                var waiter = new WaitableItem();
    //                m_waiters.Enqueue(waiter);
    //                return waiter.m_tcs.Task;
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 尝试获取锁，立刻返回
    //    /// </summary>
    //    /// <param name="owner"></param>
    //    /// <returns></returns>
    //    public bool TryEnter(Int64 ownerId)
    //    {
    //        //Int32 threadId = Thread.CurrentThread.ManagedThreadId;

    //        if (ownerId == 0)
    //        {
    //            throw new ArgumentException("ownerId can not be 0");
    //        }

    //        lock (this)
    //        {
    //            // 判断ownerid,线程id是否相同, 重入判定
    //            if (m_reenterable && ownerId == m_ownerId) // && threadId == m_threadId)
    //            {
    //                // 两个都相同可以重入, 这里有个隐含的要求一个现场不允许在同一个线程上有两个同时存在的执行现场
    //                m_reenterCount++;
    //                return true;
    //            }

    //            // 如果等于1说明是唯一当前拥有锁的现场
    //            if (Interlocked.Increment(ref m_waiterCount) == 1)
    //            {
    //                m_ownerId = ownerId;
    //                //m_threadId = threadId;
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 释放锁
    //    /// </summary>
    //    /// <param name="owner"></param>
    //    public void Leave(Int64 ownerId)
    //    {
    //        //Int32 threadId = Thread.CurrentThread.ManagedThreadId;

    //        lock (this)
    //        {
    //            // 只能允许拥有者释放锁
    //            if (ownerId != m_ownerId) //|| threadId != m_threadId)
    //            {
    //                throw new SynchronizationLockException("Lock not owned by calling thread or owner");
    //            }

    //            // 如果是某个重入的释放，直接返回
    //            if (m_reenterable && m_reenterCount != 0)
    //            {
    //                m_reenterCount--;
    //                return;
    //            }

    //            // 先重置m_threadId再m_ownerId
    //            //m_threadId = 0;
    //            m_ownerId = 0;

    //            // 释放等待计数
    //            Interlocked.Decrement(ref m_waiterCount);

    //            // 唤醒等待中的task
    //            WaitableItem toRelease;
    //            if (m_waiters.TryDequeue(out toRelease))
    //            {
    //                m_ownerId = toRelease.m_ownerId;
    //                //m_threadId = toRelease.m_threadId;
    //                toRelease.m_tcs.SetResultAsync(true);
    //            }
    //        }
    //    }

    //    private bool m_reenterable = false;
    //    private Int64 m_ownerId = 0;
    //    //private Int32 m_threadId = 0;
    //    private Int32 m_waiterCount = 0;
    //    private Int32 m_reenterCount = 0;

    //    private readonly ConcurrentQueue<WaitableItem> m_waiters = new ConcurrentQueue<WaitableItem>();
    //    private readonly static Task s_completed = Task.CompletedTask;
    //}
}
