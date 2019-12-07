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
    public class TankGameServerGlobalConfig : ServerBaseGlobalConfigure
    {

        /// <summary>
        /// 服务器配置
        /// </summary>
        [XmlElement("ServerContext")]
        public GameServerContext ServerContext { get; set; }


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

    

}
