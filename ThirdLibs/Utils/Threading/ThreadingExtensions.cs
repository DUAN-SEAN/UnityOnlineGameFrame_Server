using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlackJack.Utils
{
    /// <summary>
    /// 和线程同步相关的扩展
    /// </summary>
    public static class ThreadingExtensions
    {
        /// <summary>
        /// 为SpinLock提供 能够await的 TryEnter
        /// </summary>
        /// <param name="spinLock"></param>
        /// <param name="timeoutInMillionseconds"></param>
        /// <returns></returns>
        public static async Task<bool> TryEnter(this SpinLock spinLock, int timeoutInMillionseconds)
        {
            bool hasLock = false;
            DateTime timeLocked = DateTime.Now;
            do
        	{
                spinLock.TryEnter(ref hasLock);
                if (hasLock) return true;

                if (timeoutInMillionseconds > 0)
                    await Task.Yield();
	        } while ((DateTime.Now - timeLocked).Milliseconds < timeoutInMillionseconds);
            
            return false;
        }

        /// <summary>
        /// SetResult默认会使用调用线程执行 await 后面的代码
        /// 所以需要提供一个SetResultAsync让线程池线程执行await 后面的代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tcs"></param>
        /// <param name="result"></param>
        public static void SetResultAsync<T>(this TaskCompletionSource<T> tcs, T result, TaskScheduler taskScheduler = null)
        {
            if (taskScheduler != null)
            {
                Task.Factory.StartNew(() =>{tcs.SetResult(result);}, CancellationToken.None, TaskCreationOptions.LongRunning, taskScheduler);
            }
            else
            {
                Task.Run(() =>
                {
                    tcs.SetResult(result);
                });
            }
        }
    }
}
