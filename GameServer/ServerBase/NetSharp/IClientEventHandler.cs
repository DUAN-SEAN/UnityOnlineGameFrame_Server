using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.NetSharp
{
    /// <summary>
    /// 本地消息处理接口
    /// </summary>
    public interface ILocalMessageHandler
    {
        Task OnMessage(ILocalMessage msg);
        bool PostLocalMessage(ILocalMessage msg);
    }

    /// <summary>
    /// Incoming data, message handler.
    /// </summary>
    public interface IClientEventHandler : ILocalMessageHandler, ILockableContext
    {
        /// <summary>
        /// On data received.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="dataAvailable">缓存中有多少数据</param>
        /// <returns>The length of bytes handle, which the caller could discard.</returns>
        Task<int> OnData(byte[] buffer, int dataAvailable);

        /// <summary>
        /// Raises the disconnected event.
        /// The socket is closed before calling it.
        /// </summary>
        Task OnDisconnected();

        /// <summary>
        /// 当这个函数调用之后将不会再收到来自这个对象的任何消息
        /// Called when an exception raised on 'network data'/'local message' receiving loop.
        /// CANNOT be awaited cause it in a 'catch' block, which await statement is not permited.
        /// </summary>
        /// <param name="e">Exception, including but not limited to ObjectDisposedException, SocketException.</param>
        void OnException(Exception e);
    }
}
