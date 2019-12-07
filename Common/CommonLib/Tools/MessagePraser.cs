using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Crazy.Common;
namespace Crazy.Common
{
    /// <summary>
    /// 用于对protobuf的数据进行解析或者封装
    /// </summary>
    public class ProtobufPacker : IMessagePacker
    {
        public byte[] SerializeTo(object obj)
        {
            return ProtobufHelper.ToBytes(obj);
        }

        public void SerializeTo(object obj, MemoryStream stream)
        {
            ProtobufHelper.ToStream(obj, stream);
        }

        public object DeserializeFrom(Type type, byte[] bytes, int index, int count)
        {
            return ProtobufHelper.FromBytes(type, bytes, index, count);
        }

        public object DeserializeFrom(object instance, byte[] bytes, int index, int count)
        {
            return ProtobufHelper.FromBytes(instance, bytes, index, count);
        }

        public object DeserializeFrom(Type type, MemoryStream stream)
        {
            return ProtobufHelper.FromStream(type, stream);
        }

        public object DeserializeFrom(object instance, MemoryStream stream)
        {
            return ProtobufHelper.FromStream(instance, stream);
        }
    }
    public static class ProtobufHelper
    {
        /// <summary>
        /// 将message类型转换成字节数组
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] ToBytes(object message)
        {
            return ((Google.Protobuf.IMessage)message).ToByteArray();
        }
        /// <summary>
        /// 将message写入内存流
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream"></param>
        public static void ToStream(object message, MemoryStream stream)
        {
            ((Google.Protobuf.IMessage)message).WriteTo(stream);
        }
        /// <summary>
        /// 将字节流里的数据反序列化成message
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">读取起始位置</param>
        /// <param name="count">读取终止位置</param>
        /// <returns></returns>
        public static object FromBytes(Type type, byte[] bytes, int index, int count)
        {
            object message = Activator.CreateInstance(type);
            ((Google.Protobuf.IMessage)message).MergeFrom(bytes, index, count);
        
            return message;
        }
        /// <summary>
        /// 将message写入内存流
        /// </summary>
        /// <param name="instance">消息实例</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">读取起始位置</param>
        /// <param name="count">读取终止位置</param>
        /// <returns></returns>
        public static object FromBytes(object instance, byte[] bytes, int index, int count)
        {
            object message = instance;
            ((Google.Protobuf.IMessage)message).MergeFrom(bytes, index, count);
            return message;
        }
        /// <summary>
        /// 从内存流中读取数据
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="stream">内存流</param>
        /// <returns></returns>
        public static object FromStream(Type type, MemoryStream stream)
        {
            object message = Activator.CreateInstance(type);
            //(int)stream.Position
            ((Google.Protobuf.IMessage)message).MergeFrom(stream.ToArray());
            return message;
        }
        /// <summary>
        /// 从内存流中读取数据
        /// </summary>
        /// <param name="message">消息类型实例</param>
        /// <param name="stream">内存流</param>
        /// <returns></returns>
        public static object FromStream(object message, MemoryStream stream)
        {
            // 这个message可以从池中获取，减少gc
       
            
            ((Google.Protobuf.IMessage)message).MergeFrom(stream.ToArray(), (int)stream.Position, (int)stream.Length);
            //((Google.Protobuf.IMessage)message).MergeFrom(stream.GetBuffer());
            return message;
        }
    }

}
