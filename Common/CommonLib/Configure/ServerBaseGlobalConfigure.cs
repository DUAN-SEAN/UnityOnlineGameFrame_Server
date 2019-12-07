using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Crazy.Common
{
    /// <summary>
    /// 服务器全局配置基类
    /// </summary>
    [XmlRoot("Configure")]
    public class ServerBaseGlobalConfigure
    {

        public ServerBaseGlobalConfigure()
        {
            Global = new Global();
        }


        /// <summary>
        /// 服务器全局配置
        /// </summary>
        [XmlElement("Global")]
        public Global Global { get; set; }

    }
    public class Global
    {
        /// <summary>
        /// Network参数配置
        /// </summary>
        [XmlElement("Network")]
        public Network Network;


        /// <summary>
        /// 全局服务器列表，包括所有的
        /// </summary>
        [XmlArray("Servers"), XmlArrayItem("Server")]
        public Server[] Servers { get; set; }
    }

    public class Network
    {
        public Network()
        {
            InputBufferLen = 2048;
            OutputBufferLen = 8192;
            SocketInputBufferLen = 8192;
            SocketOutputBufferLen = 8192;
        }
        /// <summary>
        /// 接收缓冲区大小
        /// </summary>
        [XmlAttribute("InputBufferLen")]
        public Int32 InputBufferLen;

        /// <summary>
        /// 发送缓冲区大小
        /// </summary>
        [XmlAttribute("OutputBufferLen")]
        public Int32 OutputBufferLen;

        /// <summary>
        /// 对应的socket连接接受缓冲区大小
        /// </summary>
        [XmlAttribute("SocketInputBufferLen")]
        public Int32 SocketInputBufferLen;

        /// <summary>
        /// 对应的socket连接发送缓冲区大小
        /// </summary>
        [XmlAttribute("SocketOutputBufferLen")]
        public Int32 SocketOutputBufferLen;
    }
    /// <summary>
    /// 配置文件的Server节点。
    /// </summary>
    public class Server
    {
        /// <summary>
        /// 当前Server的ID。
        /// </summary>
        [XmlAttribute("Id")]
        public Int32 Id { get; set; }


        /// <summary>
        /// 当前Server的名称,必须Realm中唯一。
        /// </summary>
        [XmlAttribute("Name")]
        public String Name { get; set; }

        /// <summary>
        /// 绑定的端口服务IP。
        /// </summary>
        [XmlAttribute("IP")]
        public String EndPortIP { get; set; }

        /// <summary>
        /// 绑定的端口服务端口。
        /// </summary>
        [XmlAttribute("Port")]
        public Int32 EndPortPort { get; set; }

        /// <summary>
        /// 最大玩家人数
        /// </summary>
        [XmlAttribute("maxPlayerCtx")]
        public Int32 maxPlayerCtx { get; set; }

    }
}
