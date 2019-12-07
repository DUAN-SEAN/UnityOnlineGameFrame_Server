using BlackJack.Utils;
using Crazy.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace Crazy.NetSharp
{
    /// <summary>
    /// 本地消息继承的接口
    /// 只要是本地消息都要继承这个接口
    /// </summary>
    public interface ILocalMessage
    {
        Int32 MessageId { get; }
    }
    public static class TaskSchedulerHelper
    {
        static TaskSchedulerHelper()
        {
            QueuedTaskScheduler = new QueuedTaskScheduler();
            TaskSchedulers[0] = TaskScheduler.Default; //QueuedTaskScheduler.ActivateNewQueue(0);
            TaskSchedulers[1] = TaskScheduler.Default; //QueuedTaskScheduler.ActivateNewQueue(1);
        }

        public static readonly TaskScheduler[] TaskSchedulers = new TaskScheduler[2];
        public static readonly QueuedTaskScheduler QueuedTaskScheduler;

    }

    /// <summary>
    /// 对socket操作的封装，以及对线程模型的封装
    /// </summary>
    public class Client : IClient
    {
        public Client(TcpClient tcpClient, int priority = 1)
        {
            m_tcpClient = tcpClient;
            m_priority = priority;
            Closed = false;
            m_cancellationTokenSource = new CancellationTokenSource();
            m_cancellationToken = m_cancellationTokenSource.Token;
        }

        #region IClient implementation

        /// <summary>
        /// Disconnect this client.
        /// It is safe to run more than once.
        /// </summary>
        public void Disconnect()
        {
            if (Disconnected)
                return;

            try
            {
                m_tcpClient.Client.Shutdown(SocketShutdown.Both);
                m_tcpClient.Client.Disconnect(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client::Disconnect Exception={0}", ex);
                // The socket is closed before.
                // Simply ignore it.
            }
        }

        public bool Disconnected
        {
            get
            {
                if (m_tcpClient != null && m_tcpClient.Client != null)
                    return !m_tcpClient.Client.Connected;
                else
                    return true;
            }
        }

        /// <summary>
		/// Gets a value indicating whether this <see cref="NetSharp.Client"/> is closed.
		/// PostLocalMessage() or Send() not longer available.
		/// </summary>
		/// <value><c>true</c> if closed; otherwise, <c>false</c>.</value>
		public bool Closed { get; private set; }

        /// <summary>
        /// Close the client and the underlying socket.
        /// It is safe to run more than once.
        /// </summary>
        public void Close(bool isOnlySocket = false)
        {
            if (Closed)
                return;

            if (!isOnlySocket)
            {
                Closed = true;
            }

            try
            {
                // 如果没有断开连接先断开
                if (!Disconnected)
                {
                    Disconnect();
                }

                if (m_tcpClient != null)
                {
                    m_tcpClient.Close();
                }

                // 此时通过调用Cancel，能让client对象退出ClientWorkProc
                m_cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client::Disconnect Exception={0}", ex);
                // The socket is closed before.
                // Simply ignore it.
            }
            //finally
            //{
            //    // 调用dispose，释放资源
            //    //m_cancellationTokenSource.Dispose();
            //}
        }

        /// <summary>
        /// Send the data, whose size should less than or equal to OutgoingBufferSize in bytes.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="throwExceptionOnFail"></param>
        public async Task<bool> Send(ClientOutputBuffer data, bool throwExceptionOnFail = false)
        {
            if (Disconnected || data.m_dataLen == 0)
                return false;

            bool sendOk = true;
            try
            {
                //Log.Info("发送一条消息 Count = " + data.m_dataLen);
                await m_tcpClient.GetStream().WriteAsync(data.m_buffer, 0, data.m_dataLen, m_cancellationToken);
            }
            catch (Exception)
            {
                if (throwExceptionOnFail)
                {
                    throw;
                }

                //Console.WriteLine("Client.Send Exception! MsgId = {0}, Error={1} time={2} BufferCount={3}", BitConverter.ToInt32(data._buffer, sizeof(Int32)), ex.Message, DateTime.Now.ToString(), ClientOutputBuffer.PoolBuffCount);
                sendOk = false;
            }
            finally
            {
                //将buff放入缓存队列中
                ClientOutputBuffer.UnlockSendBuffer(data);
            }
            ///Log.Info($"发送一条消息  {sendOk}  ");
            return sendOk;
        }

        /// <summary>
        /// 将本地消息发送给Client
        /// Client接收到消息将在虚拟线程的任务1中交给玩家现场处理
        /// </summary>
        /// <param name="msg">Message.</param>
        public bool PostLocalMessage(ILocalMessage msg)
        {
            if (!Closed)
            {
                m_localMessages.Enqueue(msg);
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 发起对指定ip端口的连接，并返回client对象
        /// </summary>
        /// <returns>The client for sending message.</returns>
        /// <param name="address">Address.</param>
        /// <param name="port">Port.</param>
        /// <param name="clientEventHandler">handler for receiving messages and exceptions.</param>
        /// <param name="bufferSize">Buffer size.</param>
        /// <param name="priority"></param>
        public static Client ConnectTo(IPAddress address, int port, IClientEventHandler clientEventHandler,
            int bufferSize = DefaultInputBufferSize, int priority = 1)
        {
            var tcpClient = new TcpClient();

            // 进行连接
            // 因为外面调用的时候，采用了同步方法，所以为了避免出现死锁问题，不用async方法 2016/3/17 ouyangxiong
            tcpClient.Connect(address, port);

            // 用得到的socket对象构造client
            var client = new Client(tcpClient);

            // 启动为client服务的task
            // ClientWorkProc的执行线程，不能与SynchronizationContext的线程在同一个线程中，所以此处用Task.Run来执行，让系统线程池来执行
            //Task.Run(() => { ClientWorkProc(client, clientEventHandler, bufferSize).Wait(); }); // todo:ouyang test
            Task.Factory.StartNew(() => { ClientWorkProc(client, clientEventHandler, bufferSize).Wait(); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskSchedulerHelper.TaskSchedulers[priority]);

            if (client.Closed)
                return null;

            return client;
        }

        /// <summary>
		/// client的服务虚拟线程函数
        /// </summary>
		public static async Task ClientWorkProc(Client client, IClientEventHandler clientEventHandler,
                                                     int bufferSize)
        {
            //Socket socket = client.Socket;
            var tcpClient = client.TcpClient;
            var buffer = new byte[bufferSize];
            int bytesLeft = 0;
            List<Task> tasks;
            IClientEventHandler tempHandler;

            // 保存clientEventHandler
            client.m_clientEventHandler = clientEventHandler;

            try
            {
                var netStream = tcpClient.GetStream();

                // 构造task数组，用来进行Task.WhenAny
                tasks = new List<Task>
                {
			        // 构造取消task
			        client.MakeCancelTask(),
					// 构造消息队列task
				    client.m_localMessages.DequeueAsync(),
			        // 构造tcp关联task
			        netStream.ReadAsync(buffer, 0, buffer.Length, client.m_cancellationToken).IgnoreExceptions(),
                };

                while (!client.Closed)
                {
                    Task completedTask;
                    bool disconnectOccured = false;

                    if (client.IsPause)
                    {
                        client.WaitResume();
                    }

                    try
                    {
                        // 在task数组上等待任意一个task返回
                        completedTask = await Task.WhenAny(tasks);
                    }
                    catch (IOException)
                    {
                        // 只有socket被关闭的时候才会出现这个exception
                        if (!client.m_onDisconnectedCalled)
                        {
                            disconnectOccured = true;
                            goto LB_DISCONNECT;
                        }
                        throw;
                    }
                    catch (ObjectDisposedException)
                    {
                        // tcpclient被释放也会产生这个异常
                        if (!client.m_onDisconnectedCalled)
                        {
                            disconnectOccured = true;
                            goto LB_DISCONNECT;
                        }
                        throw;
                    }

                    switch (tasks.IndexOf(completedTask))
                    {
                        case 2:
                            int byteReceived;
                            try
                            {
                                byteReceived = await (Task<int>)completedTask;
                            }
                            catch (IOException)
                            {
                                // 只有socket被关闭的时候才会出现这个exception
                                if (!client.m_onDisconnectedCalled)
                                {
                                    disconnectOccured = true;
                                    goto LB_DISCONNECT;
                                }
                                else
                                    throw;
                            }
                            catch (ObjectDisposedException)
                            {
                                // tcpclient被释放也会产生这个异常
                                if (!client.m_onDisconnectedCalled)
                                {
                                    disconnectOccured = true;
                                    goto LB_DISCONNECT;
                                }
                                else
                                    throw;
                            }

                            // Connection closed.
                            if (byteReceived == 0)
                            {
                                disconnectOccured = true;
                                goto LB_DISCONNECT;
                            }
                   
                            int bytesHandled = 0;
                            if (!client.Disconnected)
                            {
                                bytesLeft += byteReceived;

                                // 取得锁进行回调
                                tempHandler = client.m_clientEventHandler;
                                using (await ContextLock.Create(tempHandler))
                                {
                                    //将从网络得来的数据交给玩家现场处理  
                                    bytesHandled = await client.m_clientEventHandler.OnData(buffer, bytesLeft);
                                }
                            }

                            // 检查ondata返回的参数是否正确
                            if (bytesHandled < 0 || bytesHandled > bytesLeft)
                            {
                                // 如果ondata对数据的处理有问题，直接抛出异常
                                throw new ErrorOnDataResultException(bytesHandled, bytesLeft);
                            }

                            // 将消耗掉的数据移除
                            bytesLeft -= bytesHandled;

                            if (buffer.Length == bytesLeft)
                            {
                                // 如果ondata对数据的处理有问题，直接抛出异常
                                throw new ErrorOnDataResultException(bytesHandled, bytesLeft);
                            }

                            // If any data left, move ahead.
                            if (bytesLeft > 0)
                                Array.Copy(buffer, bytesHandled, buffer, 0, bytesLeft);

                            // 重新构造tcp关联task对象

                            try
                            {
                                tasks[2] = netStream.ReadAsync(buffer, bytesLeft, buffer.Length - bytesLeft,
                                    client.m_cancellationToken).IgnoreExceptions();
                            }
                            catch (IOException)
                            {
                                // 只有socket被关闭的时候才会出现这个exception
                                if (!client.m_onDisconnectedCalled)
                                {
                                    disconnectOccured = true;
                                    goto LB_DISCONNECT;
                                }
                                throw;
                            }
                            catch (ObjectDisposedException)
                            {
                                // tcpclient被释放也会产生这个异常
                                if (!client.m_onDisconnectedCalled)
                                {
                                    disconnectOccured = true;
                                    goto LB_DISCONNECT;
                                }
                                throw;
                            }
                            break;
                        case 1:
                            {
                                // 继续从消息队列取新的消息
                                tasks[1] = client.m_localMessages.DequeueAsync();
                                var taskMessaging = (Task<ILocalMessage>)completedTask;
                                // 取得锁调用回调
                                tempHandler = client.m_clientEventHandler;
                                using (await ContextLock.Create(tempHandler))//将玩家现场锁住 如果当前玩家现场正在处理消息就等着
                                {
                                    //将本地消息丢到玩家现场处理
                                    await client.m_clientEventHandler.OnMessage(taskMessaging.Result);
                                }
                                break;
                            }
                        case 0:
                            {
                                // cancel 发生了
                                // 如果没有调用过ondisconnect，调用一下
                                if (!client.m_onDisconnectedCalled)
                                {
                                    disconnectOccured = true;
                                }
                                client.m_cancellationTokenSource = null;
                                if (!client.Closed)
                                {
                                    tasks[0] = client.MakeCancelTask();
                                }
                                break;
                            }

                    }

                LB_DISCONNECT:
                    // 处理断开连接
                    if (disconnectOccured && !client.m_onDisconnectedCalled)
                    {
                        client.m_onDisconnectedCalled = true;

                        // 获取锁调用回调
                        tempHandler = client.m_clientEventHandler;
                        using (await ContextLock.Create(tempHandler))
                        {
                            await client.m_clientEventHandler.OnDisconnected();
                        }

                        // 设置网络接收task为null，不再接受数据
                        tasks.RemoveAt(2);
                    }

                } // end while (!client.Closed)
            }
            catch (Exception e)
            {
                if (!client.Closed)
                {
                    tempHandler = client.m_clientEventHandler;
                    using (ContextLock.Create(tempHandler).Result)
                    {
                        client.m_clientEventHandler.OnException(e);
                    }
                    // 当有异常产生的时候，直接释放当前客户端，OnException将会是现场上的最后一次回调
                    client.Close();
                }
            }
        }

        /// <summary>
        /// 构造取消task
        /// </summary>
        /// <returns></returns>
        private Task MakeCancelTask()
        {
            if (m_cancellationTokenSource == null)
            {
                m_cancellationTokenSource = new CancellationTokenSource();
                m_cancellationToken = m_cancellationTokenSource.Token;
            }
            if (m_cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(m_cancellationToken);
            }
            else
            {
                //return Task.Run(() => { client.m_cancellationToken.WaitHandle.WaitOne(); });
                var tcs = new TaskCompletionSource<int>();
                m_cancellationToken.Register(() => tcs.TrySetCanceled(m_cancellationToken), useSynchronizationContext: false);
                return tcs.Task;
            }
        }

        /// <summary>
        /// 重置eventhandler，这个函数相当危险，必须在ondata车callstack中调用
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool ResetCientEventHandler(IClientEventHandler handler)
        {
            if (handler == null)
                return false;
            m_clientEventHandler = handler;

            return true;
        }

        /// <summary>
        /// 暂停该对象的数据接收功能
        /// </summary>
        public void Pause()
        {
            IsPause = true;
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        public void WaitResume()
        {
            m_pauseEvent.WaitOne();
        }

        /// <summary>
        /// 恢复该对象的数据接收功能
        /// </summary>
        public void Resume()
        {
            m_pauseEvent.Set();
            IsPause = false;
        }

        /// <summary>
        /// 获取客户端Ip地址
        /// </summary>
        /// <returns></returns>
        public string GetClientIp()
        {
            if (m_clientIp != null)
                return m_clientIp;

            if (m_tcpClient == null || !m_tcpClient.Connected)
                return null;

            string strIp = ((IPEndPoint)m_tcpClient.Client.RemoteEndPoint).Address.ToString();
            Interlocked.CompareExchange(ref m_clientIp, strIp, null);
            return m_clientIp;
        }

        /// <summary>
        /// 获取客户端端口
        /// </summary>
        /// <returns></returns>
        public Int32 GetClientPort()
        {
            if (m_tcpClient == null || !m_tcpClient.Connected)
                return 0;

            if (m_clientPort == 0)
                m_clientPort = ((IPEndPoint)m_tcpClient.Client.RemoteEndPoint).Port;

            return m_clientPort;
        }

        /// <summary>
        /// 设置socket的接收缓冲区大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSocketRecvBufferSize(Int32 size)
        {
            m_tcpClient.ReceiveBufferSize = size;
        }

        /// <summary>
        /// 设置socket的发送缓冲区大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSocketSendBufferSize(Int32 size)
        {
            m_tcpClient.SendBufferSize = size;
        }

        /// <summary>
        /// 底层的tcp连接对象
        /// </summary>
        private readonly TcpClient m_tcpClient;
        public TcpClient TcpClient { get { return m_tcpClient; } }

        /// <summary>
        /// 优先级
        /// </summary>
        private int m_priority;

        /// <summary>
        /// 事件处理对象，实际对象为PlayerContextBase类
        /// </summary>
        private IClientEventHandler m_clientEventHandler;

        /// <summary>
        /// 内部消息的列表，保存在一个awaitable的队列中
        /// </summary>
        private AwaitableQueue<ILocalMessage> m_localMessages = new AwaitableQueue<ILocalMessage>();

        /// <summary>
        /// 连接的对象ip和端口信息
        /// </summary>
        private String m_clientIp;
        private Int32 m_clientPort;
        private const Int32 DefaultInputBufferSize = 2048;

        /// <summary>
        /// onDisconnected 回调是否调用过了
        /// </summary>
        private bool m_onDisconnectedCalled;

        /// <summary>
        /// 用来终止ClientWorkProc循环
        /// </summary>
        private CancellationTokenSource m_cancellationTokenSource;
        private CancellationToken m_cancellationToken;

        /// <summary>
        /// 当前是否处于暂停消息接收暂停
        /// </summary>
        public bool IsPause { get; private set; }
        /// <summary>
        /// 用来暂停和恢复消息接收的信号
        /// </summary>
        private AutoResetEvent m_pauseEvent = new AutoResetEvent(false);
    }

    public class ErrorOnDataResultException : Exception
    {
        public ErrorOnDataResultException(int bytesHandled, int bytesLeft) :
            base(string.Format("Invalidated return value of OnData:{0}, should less than {1}.",
                bytesHandled, bytesLeft))
        {

        }
    }
}
