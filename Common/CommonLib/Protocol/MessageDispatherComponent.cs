using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
namespace Crazy.Common
{
    /// <summary>
    /// 消息分发类型，将网络消息和handler进行绑定
    ///
    /// </summary>
    public class MessageDispather
    {
        /// <summary>
        /// Load 在组件启动时调用
        /// </summary>
        public MessageDispather()
        {
            m_instance = this;
        }
        public bool Init()
        {
            Handlers.Clear();

            // AppType appType = StartConfigComponent.Instance.StartConfig.AppType;

            List<Type> types = TypeManager.Instance.GetTypes(typeof(MessageHandlerAttribute));

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                MessageHandlerAttribute messageHandlerAttribute = attrs[0] as MessageHandlerAttribute;
                //if (!messageHandlerAttribute.Type.Is(appType))
                //{
                //    continue;
                //}

                if (!(Activator.CreateInstance(type) is IMHandler iMHandler))
                {
                    Log.Error($"message handle {type.Name} 需要继承 IMHandler");
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();//获取消息的类型
                ushort opcode = OpcodeTypeDictionary.Instance.GetIdByType(messageType);
                if (opcode == 0)
                {
                    Log.Error($"消息opcode为0: {messageType.Name}");
                    continue;
                }
                RegisterHandler(opcode, iMHandler);
            }
            Log.Info("Handler Count = " + Handlers.Values.Count);
            return true;
        }
        public void RegisterHandler(ushort opcode, IMHandler handler)
        {
            if (!Handlers.ContainsKey(opcode))
            {
                Handlers.Add(opcode, new List<IMHandler>());
                //Log.Debug(opcode.ToString());
            }
            //Log.Debug(opcode.ToString());
            Handlers[opcode].Add(handler);
        }

        public void Handle(ISession sender, MessageInfo messageInfo)
        {
            if (!Handlers.TryGetValue(messageInfo.Opcode, out var handlers))
            {
                Log.Error($"消息没有处理:{messageInfo.Opcode} {messageInfo.Message}");

                return;
            }

            foreach (IMHandler ev in handlers)
            {
                try
                {

                    ev.Handle(sender, messageInfo.Message);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public readonly Dictionary<ushort, List<IMHandler>> Handlers = new Dictionary<ushort, List<IMHandler>>();

        private static  MessageDispather m_instance;
        public static MessageDispather Instance { get => m_instance; }
    }
}
