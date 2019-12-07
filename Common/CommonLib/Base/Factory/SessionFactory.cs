using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crazy.Common
{
    /// <summary>
    /// sessionid生成工厂
    /// </summary>
    [Serializable]
    public class SessionIdFactory
    {
        public SessionIdFactory(Int32 serverId)
        {
            m_serverId = (UInt64)serverId;
        }

        public UInt64 AllocateSessionId()
        {
            return (m_serverId << 32) + (UInt32)Interlocked.Increment(ref m_ctxId);//线程安全的递增
        }

        public static Int32 GetServerIdFromSessionId(UInt64 sessionId)
        {
            return (Int32)(sessionId >> 32);
        }
        public static Int32 GetContextIdFromSessionId(UInt64 sessionId)
        {
            return (Int32)(sessionId & 0x00000000ffffffff);
        }

        private UInt64 m_serverId;
        private Int32 m_ctxId = 0;
    }
}
