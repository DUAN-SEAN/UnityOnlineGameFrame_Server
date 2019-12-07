using Crazy.Common;
using System;
using System.Collections.Generic;
using System.Text;
namespace Crazy.Common
{
  
    public abstract class AMHandler<Message> : IMHandler where Message : class 
    {
        protected abstract void Run(ISession playerContext, Message message);
        public Type GetMessageType()
        {
            return typeof(Message);
        }

        public void Handle(ISession sender, object msg)
        {
            if (!(msg is Message message))
            {
                Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(Message).Name}");
                return;
            }
            if (sender.SessionId == 0)
            {
                Log.Error("Session ID is 0");
                return;
            }
            Run(sender, message);
            //MessageFactory.Recycle(message as IMessage);
        }
    }

    public abstract class AMRpcHandler<Request, Response> : IMHandler 
        where Request : class, IRequest where Response : class, IResponse
    {
        protected static void ReplyError(Response response, Exception e, Action<Response> reply)
        {
            Log.Error(e);
            response.Error = ErrorCode.ERR_RpcFail;
            response.Message = e.ToString();
            reply(response);
        }

        protected abstract void Run(ISession playerContext, Request message, Action<Response> reply);

        public void Handle(ISession sender, object message)
        {
            try
            {
                Request request = message as Request;
                
                if (request == null)
                {
                    Log.Error($"消息类型转换错误: {message.GetType().Name} to {typeof(Request).Name}");
                }

                int rpcId = request.RpcId;

                ulong instanceId = sender.SessionId;

                this.Run(sender, request, response =>
                {
                    // 等回调回来,session可以已经断开了,所以需要判断session InstanceId是否一样
                    if (sender.SessionId!= instanceId)
                    {
                        Log.Error("Session Id is FAIL");
                        return;
                    }

                    response.RpcId = rpcId;
                    sender.Reply(response);
                });
                //MessageFactory.Recycle(request as IMessage);//2019 7 17 更新对象池
            }
            catch (Exception e)
            {
                throw new Exception($"解释消息失败: {message.GetType().FullName}", e);
            }
        }

        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }

}
