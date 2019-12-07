using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Utils
{
    /// <summary>
    /// 可锁现场接口
    /// </summary>
    public interface ILockableContext
    {
        /// <summary>
        /// 进入锁
        /// </summary>
        /// <returns></returns>
        Task EnterLock();
        /// <summary>
        /// 离开锁
        /// </summary>
        void LeaveLock();

        /// <summary>
        /// 获取现场的实例id
        /// </summary>
        /// <returns></returns>
        Int64 GetInstanceId();
    }

    /// <summary>
    /// 为了以using的方式使用可锁现场的辅助类
    /// </summary>
    public class ContextLock : IDisposable
    {
        public static async Task<ContextLock> Create(ILockableContext ctx)
        {
            await ctx.EnterLock();
            return new ContextLock(ctx);
        }

        private ContextLock(ILockableContext ctx)
        {
            m_ctx = ctx;
        }

        public void Dispose()
        {
            m_ctx.LeaveLock();
        }

        private ILockableContext m_ctx = null;
    }
}
