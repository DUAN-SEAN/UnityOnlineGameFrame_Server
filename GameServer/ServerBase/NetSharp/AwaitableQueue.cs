using BlackJack.Utils;
using System.Collections.Concurrent;
using System.Threading.Tasks;


namespace Crazy.NetSharp
{
    /// <summary>
    /// 可以await的队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AwaitableQueue<T>
    {
		/// <summary>
		/// Enqueue the specified item. Blocking operation.
		/// </summary>
		/// <param name="item">Item.</param>
        public void Enqueue(T item)
        {
            if (m_isShutdown)
            {
                return;
            }

            TaskCompletionSource<T> result;
            lock (m_lock)
            {
                if (m_penddingTaskQueue.TryDequeue(out result))
                {
                    result.SetResultAsync(item);
                }
                else
                {
                    m_queue.Enqueue(item);
                }
            }
        }

        public async Task<T> DequeueAsync()
        {
            T result = default(T);

            if (m_isShutdown)
            {
                return result;
            }

            TaskCompletionSource<T> tcs;
            lock (m_lock)
            {
                if (m_queue.TryDequeue(out result))
                    return result;

                tcs = new TaskCompletionSource<T>();
                m_penddingTaskQueue.Enqueue(tcs);
            }

            return tcs.Task.IsCompleted ? tcs.Task.Result : await tcs.Task;
        }

        public bool TryDequeue(out T t)
        {
            return m_queue.TryDequeue(out t);
        }

        /// <summary>
        /// 取消所有正在进行的等待
        /// </summary>
        public void Shutdown()
        {
            m_isShutdown = true;

            while (true)
            {
                TaskCompletionSource<T> tcs;
                if (m_penddingTaskQueue.TryDequeue(out tcs))
                {
                    tcs.SetCanceled();
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 队列的长度
        /// </summary>
        public int Count
        {
            get
            {
                return m_queue.Count;
            }
        }

        private readonly ConcurrentQueue<T> m_queue = new ConcurrentQueue<T>();
        private readonly ConcurrentQueue<TaskCompletionSource<T>> m_penddingTaskQueue = new ConcurrentQueue<TaskCompletionSource<T>>();
        private bool m_isShutdown = false;
        private object m_lock = new object();
    }
}
