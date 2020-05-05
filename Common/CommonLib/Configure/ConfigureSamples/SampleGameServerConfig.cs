using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Crazy.Common;

namespace GameServer.Configure
{
    [Serializable]
    [XmlRoot("Configure")]
    public class SampleGameServerGlobalConfig : ServerBaseGlobalConfigure
    {
        /// <summary>
        /// 服务器数据库列表
        /// </summary>
        [XmlArray("ServerTypes"), XmlArrayItem("ServerType")]
        public ServerType[] ServerTypes { get; set; }
        /// <summary>
        /// 服务器配置
        /// </summary>
        [XmlElement("ServerContext")]
        public ServerContext ServerContext { get; set; }


        /// <summary>
        /// 服务器数据库列表
        /// </summary>
        [XmlArray("DBConfig"), XmlArrayItem("Database")]
        public DBConfigInfo[] DBConfigInfos { get; set; }
       
        /// <summary>
        /// 玩家现场配置
        /// </summary>
        [XmlElement("GameServerPlayerContext")]
        public GameServerPlayerContext GameServerPlayerContext { get; set; }

      




    }

    [Serializable]
    public class ServerContext
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("AsyncActionQueueCount")]
        public UInt32 AsyncActionQueueCount { get; set; }

        /// <summary>
        /// 心跳包的发送间隔
        /// </summary>
        [XmlAttribute("HeartBeatTimerPeriod")]
        public int HeartBeatTimerPeriod { get; set; }

    }
    [Serializable]
    public class ServerType
    {


        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }
    }
}
