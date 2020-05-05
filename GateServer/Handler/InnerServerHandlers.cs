using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Google.Protobuf;
using IMessage = Crazy.Common.IMessage;

namespace GateServer
{
    [MessageHandler]
    public class S2G_WarppedServerMessageHandler:AMHandler<S2G_WarppedServerMessage>
    {
        protected override void Run(ISession playerContext, S2G_WarppedServerMessage message)
        {
            BDGameServerPlayerContext context =  GateServer.Instance.PlayerCtxManager.FindPlayerContextByString(message.PlayerId) as BDGameServerPlayerContext;
            if (context == null)
            {
                Log.Info("BDGameServerPlayerContext is NULL");
                return;
            }

            Type t = GateServer.Instance.OpcodeTypeDic.GetTypeById((ushort)message.TypeId);
            var msg = ProtobufHelper.FromBytes(t, message.MessageBody.ToByteArray(),0,message.MessageBody.Length) as IMessage;
            context.Send(msg);
        }
    }
}
