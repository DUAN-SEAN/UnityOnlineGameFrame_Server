using Crazy.ServerBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
namespace Crazy.NetSharp
{
    /// <summary>
    /// 用来提供网络服务，提供Ip和端口服务
    /// 
    /// 服务器使用TcpListener进行端口监听
    /// 并用TcpClient 与客户端进行面向连接
    /// </summary>
    public class Service
    {


        public Service(int incomingBufferSize = DefaultBufferSize, int priority = 1)
        {
            m_priority = priority;
            IncomingBufferSize = incomingBufferSize;
        }
        /// <summary>
        /// 启动服务 并将GameServer交给Service回调OnConeection and OnException
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="handler"></param>
        public void Start(IPAddress address, int port, IServiceEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(handler.GetType().FullName);
            }

            m_serviceEventHandler = handler;

            // 启动监听
            m_listener = new TcpListener(address, port);
            m_listener.Start();

            // 监听成功开启后开始持续工作
            m_state = State.Running;
            var result = new Task
            (
                () => { ServiceStart().Wait(); },
                TaskCreationOptions.LongRunning
            );
            result.ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);//发生错误就通知ServerBase
            result.Start(TaskSchedulerHelper.TaskSchedulers[m_priority]);
            m_mainTask = result;


        }
        /// <summary>
        /// 开启服务
        /// </summary>
        public async Task ServiceStart()
        {
            

            while (m_state == State.Running)
            {
                //Socket socket;
                TcpClient tcpClient;
                try
                {
                    // 得到一条新的连接 
                    tcpClient = await m_listener.AcceptTcpClientAsync();
                    tcpClient.SendTimeout = SocketTimeout;
                    tcpClient.ReceiveTimeout = SocketTimeoutMax;
                }
                catch (ObjectDisposedException)
                {
                    // 这里是由于listener.stop()被调用
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("ServiceWork {0}:{1}:{2}", ex.GetType(), ex.ErrorCode, ex.Message);
                    if (ex.ErrorCode == 10054)
                    {
                        // 这种情况是由于链接的三次握手还没有完成，对方接断开连接导致的异常 errcode 10054
                        continue;
                    }
                    throw;
                }
                // ReSharper disable once RedundantCatchClause
                catch (Exception)
                {
                    // 任何其他exception直接抛给调用者
                    throw;
                }

                // 启动一个task为新连接服务
                // ReSharper disable once UnusedVariable
                Task noWarning = Task.Run(() => ServiceProc(tcpClient)).ContinueWith(OnClientException, TaskContinuationOptions.OnlyOnFaulted);
            }

            // 设置_quitFlag为退出完成
            m_state = State.Exited;
        }

        private async void ServiceProc(TcpClient tcpClient)
        {
            if (tcpClient == null)
                return;
            Client client = new Client(tcpClient, m_priority);
            // 通知应用层OnConnect
            if (m_serviceEventHandler == null)
            {
                Log.Error("m_serviceEventHandler null");
                return;
            }
                
            //通知Server注册玩家 返回玩家现场
            var clientEventHandler = await m_serviceEventHandler.OnConnect(client);
            if (clientEventHandler == null)
            {
                // 断开连接，释放已经分配的资源
                client.Close();
                return;
            }

            // 将活动的client对象记录下来
            m_clientActiveness.TryAdd(client, true);

            await Client.ClientWorkProc(client, clientEventHandler, IncomingBufferSize);

            // 当ClientWorkProc结束的时候说明client对象已经关闭
            bool activeness;
            // 从注册表中将client对象移除
            m_clientActiveness.TryRemove(client, out activeness);
        }

        /// <summary>
        /// 关闭当前endpoint，调用者在调用该函数之前需要首先进行应用层的关闭来释放所有的client对象
        /// </summary>
        public void Stop()
        {
            // 停止监听
            m_listener.Stop();

            // 设置退出标志
            if (m_state == State.Running)
                m_state = State.Exiting;
        }

        /// <summary>
        /// 客户端调用此方法与服务器进行连接
        /// 或者服务器调用此方法与客户端进行连接
        /// </summary>
        public void Connect()
        {

        }

        /// <summary>
        /// Logs the exception which stopped the task.
        /// </summary>
        /// <param name="t">T.</param>
        private void OnException(Task t)
        {
    
            Exception e = t.Exception;
            Log.Error($"Exception unhandled in client mainloop task: { e.Message}");
            m_serviceEventHandler.OnException(e);
        }
        private void OnClientException(Task t)
        {

            Exception e = t.Exception;
            Log.Error($"Exception unhandled in client mainloop task: { e.Message}");
        }
        // 默认的数据接收缓冲区大小
        public const int DefaultBufferSize = 1024 * 32;
        /// <summary>
        /// 数据接收缓冲区大小
        /// Gets or sets the size of the incoming buffer per client.
        /// Updates ONLY effect on new connections.
        /// </summary>
        /// <value>The size of the buffer.</value>
        public int IncomingBufferSize { get; private set; }
        /// <summary>
        /// TCP监听对象
        /// </summary>
        private TcpListener m_listener;
        /// <summary>
        /// 服务根Task
        /// </summary>
        private Task m_mainTask;
        /// <summary>
        /// 用来保存所有目前还在被上层使用的client对象
        /// </summary>
		private ConcurrentDictionary<Client, bool> m_clientActiveness = new ConcurrentDictionary<Client, bool>();
        private enum State
        {
            ///  0 没有退出			
            Running = 0,
            ///  1 正在退出，该状态之下不会再接收新的连接
			Exiting,
            ///  2 退出执行完成
            Exited
        }
        /// <summary>
        /// 用来标识退出状态
        /// </summary>
        private State m_state = State.Running;
        /// <summary>
        /// Server提供的处理事件句柄
        /// </summary>
        private IServiceEventHandler m_serviceEventHandler;
        /// <summary>
        /// socket 超时设置
        /// </summary>
        private const Int32 SocketTimeout = 30000;

        /// <summary>
        /// socket 超时设置无限大
        /// </summary>
        private const Int32 SocketTimeoutMax = 0;

        /// <summary>
        /// 优先级
        /// </summary>
        private int m_priority;
    }
}
