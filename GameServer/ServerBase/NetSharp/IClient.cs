using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.NetSharp
{
    /// <summary>
    /// 服务器消息封装
    /// </summary>
    public class ClientOutputBuffer
    {
        public ClientOutputBuffer()
        {
            m_maxLen = m_maxLenDefault;
            m_buffer = new byte[m_maxLenDefault];
        }

        public ClientOutputBuffer(int size)
        {
            m_maxLen = size;
            m_buffer = new byte[m_maxLen];
        }

        /// <summary>
        /// [ThreadSafe]
        /// 获取buf
        /// </summary>
        public static ClientOutputBuffer LockSendBuffer(int size)
        {
            if (!m_outputBufferPool.TryPop(out var buf))
            {
                buf = new ClientOutputBuffer(size);
                m_lastLockTime = DateTime.Now;
            }
            else
            {
                //如果池子里没有则 新生成一个 如果长度不够，重新分配一下
                if (buf.m_maxLen < size)
                {
                    Array.Resize(ref buf.m_buffer, size);
                    buf.m_maxLen = size;
                }
            }

            if (m_outputBufferPool.Count == 0)
            {
                m_lastLockTime = DateTime.Now;
            }
            return buf;
        }
        /// <summary>
        /// 获取buf
        /// </summary>
        /// <returns></returns>
        public static ClientOutputBuffer LockSendBuffer()
        {
            return LockSendBuffer(m_maxLenDefault);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="buf"></param>
        public static void UnlockSendBuffer(ClientOutputBuffer buf)
        {
            //如果当前池子的buff过多就清理一次缓存
            if (m_outputBufferPool.Count > m_maxPoolLength)
            {
                if ((DateTime.Now - m_lastLockTime).TotalMinutes > m_maxPoolFreeWaitMinutes)
                {
                    // 清理数据
                    ClientOutputBuffer outBuf = null;
                    while (m_outputBufferPool.Count > m_maxPoolLength)
                    {
                        m_outputBufferPool.TryPop(out outBuf);
                    }
                }
            }

            buf.m_dataLen = 0;
            m_outputBufferPool.Push(buf);
        }

        /// <summary>
        /// 默认发送缓存大小
        /// </summary>
        public static int m_maxLenDefault = 8 * 1024;//16位

        /// <summary>
        /// 最大的pool长度
        /// </summary>
        public static int m_maxPoolLength = 2000;

        /// <summary>
        /// 释放pool的等待时间
        /// </summary>
        public static int m_maxPoolFreeWaitMinutes = 5;

        /// <summary>
        /// 可设定的发送缓存大小
        /// </summary>
        private int m_maxLen;

        /// <summary>
        /// 发送缓存
        /// </summary>
        public byte[] m_buffer;

        /// <summary>
        /// 发送缓存实际数据长度
        /// </summary>
        public int m_dataLen;

        /// <summary>
        /// 缓存池中buffer的个数
        /// </summary>
        public static int PoolBuffCount { get { return m_outputBufferPool.Count; } }

        /// <summary>
        /// 缓存池
        /// </summary>
        private static ConcurrentStack<ClientOutputBuffer> m_outputBufferPool = new ConcurrentStack<ClientOutputBuffer>();

        private static DateTime m_lastLockTime;
    }
    /// <summary>
    /// 本地消息接收者
    /// </summary>
    public interface ILocalMessageClient
    {
        // 向消息队列发送消息
        bool PostLocalMessage(ILocalMessage msg);
    }
    public interface IClient:ILocalMessageClient
    {
        // 发起对连接的断开
        void Disconnect();
        // 是否已经断开。
        bool Disconnected { get; }
        // 关闭当前客户端对象，在Disconnect之后调用
        void Close(bool onlySocket = false);
        // 在这里实现异步的发送
        Task<bool> Send(ClientOutputBuffer data, bool throwExceptionOnFail = false);
        // 关闭连接，在Disconnect之后调用
        bool Closed { get; }
        // 重新设置clienteventhandler
        bool ResetCientEventHandler(IClientEventHandler handler);
        // 获取客户端Ip
        string GetClientIp();
        // 获取客户端端口
        int GetClientPort();
        // 设置TCP连接的ReceiveBufferSize
        void SetSocketRecvBufferSize(int size);
        // 设置TCP连接的SendBufferSize
        void SetSocketSendBufferSize(int size);


    }
}
