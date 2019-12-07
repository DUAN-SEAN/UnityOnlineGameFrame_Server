using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crazy.Common;
namespace Crazy.Common
{
    public interface ISession
    {

        void Reply(IResponse message);
        //一次发送多个消息
        int Send(List<IMessage> messages);
        int Send(IMessage message);
        Task<IResponse> Call(IRequest request);

        Dictionary<int, Action<IResponse>> GetRPCActionDic();

        ulong SessionId { get; }
    }

   
}
