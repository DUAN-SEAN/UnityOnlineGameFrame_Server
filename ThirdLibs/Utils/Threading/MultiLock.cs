using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace BlackJack.Utils
{
    /// <summary>
    /// 多目标锁
    /// </summary>
    public class MultiLock : IDisposable
    {
        /// <summary>
        /// 支持using(MultiLock.Create()){}使用
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static MultiLock Create(Object obj1, Object obj2)
        {
            Debug.Assert(obj1 != null, "MultiLock:Create obj1 is null");
            Debug.Assert(obj2 != null, "MultiLock:Create obj2 is null");

            Enter(obj1, obj2);
            return new MultiLock(obj1, obj2);
        }

        private MultiLock(Object obj1, Object obj2)
        {
            m_obj1 = obj1;
            m_obj2 = obj2;
        }

        /// <summary>
        /// 按照顺序对两个obj进行锁定
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static void Enter(Object obj1, Object obj2)
        {
            if (obj1.GetHashCode() >= obj2.GetHashCode())
            {
                Monitor.Enter(obj1);
                Monitor.Enter(obj2);
            }
            else
            {
                Monitor.Enter(obj2);
                Monitor.Enter(obj1);
            }
        }

        /// <summary>
        /// 在给定时间millisecondsTimeout内，按照顺序对两个obj进行锁定
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public static bool TryEnter(Object obj1, Object obj2, int millisecondsTimeout)
        {
            Debug.Assert(obj1 != null, "MultiLock:TryEnter obj1 is null");
            Debug.Assert(obj2 != null, "MultiLock:TryEnter obj2 is null");
            Debug.Assert(millisecondsTimeout >= 0 || millisecondsTimeout == Timeout.Infinite, "millisecondsTimeout is minus and not equals System.Threading.Timeout.Infinite");

            if (obj1.GetHashCode() >= obj2.GetHashCode())
            {
                if (Monitor.TryEnter(obj1, millisecondsTimeout))
                {
                    if (Monitor.TryEnter(obj2, millisecondsTimeout))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Monitor.TryEnter(obj2, millisecondsTimeout))
                {
                    if (Monitor.TryEnter(obj1, millisecondsTimeout))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 按照顺序解除对两个obj的锁定
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static void Exit(Object obj1, Object obj2)
        {
            if (obj1.GetHashCode() >= obj2.GetHashCode())
            {
                // exit的调用顺序与enter相反
                Monitor.Exit(obj2);
                Monitor.Exit(obj1);
            }
            else
            {
                Monitor.Exit(obj1);
                Monitor.Exit(obj2);
            }
        }

        #region IDisposable 成员
        public void Dispose()
        {
            Exit(m_obj1, m_obj2);
        }
        #endregion

        private object m_obj1;
        private object m_obj2;
    }
}
