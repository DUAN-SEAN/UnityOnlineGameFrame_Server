using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlackJack.Utils
{
    /// <summary>
    /// 为了以using的方式使用SemaphoneSlim
    /// 
    /// using(SemaphoneSlimLock.Create(semaphore))
    /// {
    ///     ...
    /// }
    /// </summary>
    public class SemaphoreSlimLock : IDisposable
    {
        public static async Task<SemaphoreSlimLock> Create(SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new SemaphoreSlimLock(semaphore);
        }

        private SemaphoreSlimLock(SemaphoreSlim semaphore)
        {
            m_semaphore = semaphore;
        }

        public void Dispose()
        {
            m_semaphore.Release();
        }

        private SemaphoreSlim m_semaphore = null;
    }
}
