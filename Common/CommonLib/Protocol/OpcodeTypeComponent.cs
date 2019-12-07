using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
namespace Crazy.Common
{
    /// <summary>
    /// 所有走网络的消息类型都在此加载
    /// </summary>
    public class OpcodeTypeDictionary
    {
        

        public OpcodeTypeDictionary()
        {
            m_instance = this;
        }

        /// <summary>
        /// 初始化message type 
        /// </summary>
        public bool Init()
        {
            this.opcodeTypes.Clear();
            this.typeMessages.Clear();

            var types = TypeManager.Instance.GetTypes(typeof(MessageAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                var messageAttribute = attrs[0] as MessageAttribute;
                if (messageAttribute == null)
                {
                    continue;
                }

                this.opcodeTypes.Add(messageAttribute.Opcode, type);
                this.typeMessages.Add(messageAttribute.Opcode, Activator.CreateInstance(type));
                //Log.Info(((Google.Protobuf.IMessage)(Activator.CreateInstance(type))).Descriptor.FullName);
                //var message = Activator.CreateInstance(type) as Google.Protobuf.IMessage;
                //foreach (var field in message.Descriptor.Fields.InDeclarationOrder())
                //{
                //    Console.WriteLine(
                //        "Field {0} ({1}): {2}",
                //        field.FieldNumber,
                //        field.Name,
                //        field.Accessor.GetValue(message));
                //}
             
     

            }
            Log.Info("Message Count = " + opcodeTypes.Values.Count);
            return true;
        }

        public ushort GetIdByType(Type type)
        {
            return opcodeTypes.GetKeyByValue(type);
        }

        public Type GetTypeById(ushort msgId)
        {
            return opcodeTypes.GetValueByKey(msgId);
        }
        // 客户端为了0GC需要消息池，服务端消息需要跨协程不需要消息池
        public object GetInstance(ushort opcode)
        {
            //暂时先不用控制GC的方式
			var type = this.GetTypeById(opcode);
            Google.Protobuf.IMessage  message = typeMessages[opcode]as Google.Protobuf.IMessage;
			return Activator.CreateInstance(type);

        }
        private static OpcodeTypeDictionary m_instance;

        public static OpcodeTypeDictionary Instance
        {
            get
            {
                return m_instance;
            }
        }

        /// <summary>
        /// 协议 和 消息类型双映射
        /// </summary>
        private readonly DoubleMap<ushort, Type> opcodeTypes = new DoubleMap<ushort, Type>();
        /// <summary>
        /// 协议和消息类型的实例字典
        /// Key 协议 ； value 消息类型实例
        /// </summary>
        private readonly Dictionary<ushort, object> typeMessages = new Dictionary<ushort, object>();

    }
}
