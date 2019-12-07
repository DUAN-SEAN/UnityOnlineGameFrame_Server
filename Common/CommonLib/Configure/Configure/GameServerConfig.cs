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
    public class GameServerGlobalConfig : ServerBaseGlobalConfigure
    {

        /// <summary>
        /// 服务器配置
        /// </summary>
        [XmlElement("ServerContext")]
        public GameServerContext ServerContext { get; set; }

        [XmlElement("BattleConfig")]
        public GameBattleConfig GameBattleConfig { get; set; }

        /// <summary>
        /// 服务器数据库列表
        /// </summary>
        [XmlArray("DBConfig"), XmlArrayItem("Database")]
        public DBConfigInfo[] DBConfigInfos { get; set; }
        /// <summary>
        /// 关卡配置
        /// </summary>
        [XmlArray("BarrierConfig"), XmlArrayItem("Barrier")]
        public GameBarrierConfig[] BarrierConfigs { get; set; }
        /// <summary>
        /// 匹配队伍数
        /// </summary>
        [XmlElement("GameMatchTeam")]
        public GameMatchTeamConfig GameMatchTeam { get; set; }
        /// <summary>
        /// 玩家现场配置
        /// </summary>
        [XmlElement("GameServerPlayerContext")]
        public GameServerPlayerContext GameServerPlayerContext { get; set; }

        /// <summary>
        /// 关卡配置
        /// </summary>
        [XmlArray("ShipConfig"), XmlArrayItem("Ship")]
        public GameShipConfig[] GameShipConfig { get; set; }
        /// <summary>
        /// 技能配置
        /// </summary>
        [XmlElement("GameSkillConfig")]
        public GameSkillConfig GameSkillConfig { get; set; }




    }

    [Serializable]
    public class GameBattleConfig
    {
        [XmlAttribute("TickTime")]
        public int BattleTickTime { get; set; }

        [XmlAttribute("LevelTickTime")]
        public int LevelTickTime { get; set; }
    }

    [Serializable]
    public class GameSkillConfig
    {
        [XmlArray("DamageSkillConfig"), XmlArrayItem("Damage")]
        public DamageSkillConfig[] DamageSkillConfig { get; set; }

        [XmlArray("SummonSkillConfig"), XmlArrayItem("Summon")]
        public SummonSkillConfig[] SummonSkillConfig { get; set; }

        [XmlArray("RecoverySkillConfig"), XmlArrayItem("Recovery")]
        public RecoverySkillConfig[] RecoverySkillConfig { get; set; }

        [XmlArray("GainSkillConfig"), XmlArrayItem("Gain")]
        public GainSkillConfig[] GainSkillConfig { get; set; }


    }
    [Serializable]
    public class GainSkillConfig
    {
        [XmlAttribute("SkillType")]
        public Int32 SkillType { get; set; }

        [XmlAttribute("SkillName")]
        public String SkillName { get; set; }

        [XmlAttribute("CD")]
        public Int32 CD { get; set; }

        [XmlAttribute("GainType")]
        public Int32 RecoveryType { get; set; }

        [XmlAttribute("GainValue")]
        public Int32 RecoveryValue { get; set; }


    }
    [Serializable]
    public class RecoverySkillConfig
    {
        [XmlAttribute("SkillType")]
        public Int32 SkillType { get; set; }

        [XmlAttribute("SkillName")]
        public String SkillName { get; set; }

        [XmlAttribute("CD")]
        public Int32 CD { get; set; }

        [XmlAttribute("RecoveryType")]
        public Int32 RecoveryType { get; set; }

        [XmlAttribute("RecoveryValue")]
        public Int32 RecoveryValue { get; set; }



    }
    [Serializable]
    public class SummonSkillConfig
    {
        [XmlAttribute("SkillType")]
        public Int32 SkillType { get; set; }

        [XmlAttribute("SkillName")]
        public String SkillName { get; set; }

        [XmlAttribute("CD")]
        public Int32 CD { get; set; }

        [XmlAttribute("MaxSummonCount")]
        public Int32 MaxSummonCount { get; set; }

        [XmlAttribute("SummonShipType")]
        public Int32 SummonShipType { get; set; }

        [XmlAttribute("SummonSurvivalTime")]
        public Int32 SummonSurvivalTime { get; set; }

    }
    [Serializable]
    public class DamageSkillConfig
    {
        [XmlAttribute("SkillType")]
        public Int32 SkillType { get; set; }

        [XmlAttribute("SkillName")]
        public String SkillName { get; set; }

        [XmlAttribute("CD")]
        public Int32 CD { get; set; }

        [XmlAttribute("MaxCount")]
        public Int32 MaxCount { get; set; }

        [XmlAttribute("MaxDamageValue")]
        public Int32 DamageValue { get; set; }

        [XmlAttribute("DamageDistance")]
        public Int32 DamageDistance { get; set; }

        [XmlAttribute("DamageRange")]
        public Int32 DamageRange { get; set; }

        [XmlAttribute("AttackInterval")]
        public Int32 AttackInterval { get; set; }

        [XmlAttribute("PhsicsValue")]
        public Int32 PhsicsValue { get; set; }

    }


    [Serializable]
    public class GameShipConfig
    {
        [XmlAttribute("ShipType")]
        public Int32 ShipType { get; set; }

        [XmlAttribute("ShipName")]
        public String ShipName { get; set; }

        [XmlAttribute("Mass")]
        public Int32 Mass { get; set; }

        [XmlAttribute("MaxHp")]
        public Int32 MaxHp { get; set; }

        [XmlAttribute("MaxShield")]
        public Int32 MaxShield { set; get; }

        [XmlAttribute("ShieldRecoverySpeed")]
        public Int32 ShieldRecoverySpeed { set; get; }

        [XmlAttribute("AccelerationSpeed")]
        public Int32 AccelerationSpeed { set; get; }

        [XmlAttribute("MaxAccelerationSpeed")]
        public Int32 MaxAccelerationSpeed { set; get; }

        [XmlAttribute("MaxSpeed")]
        public Int32 MaxSpeed { set; get; }

        [XmlAttribute("MaxTurnSpeed")]
        public Int32 MaxTurnSpeed { set; get; }

        [XmlAttribute("WeaponOne")]
        public Int32 WeaponOne { set; get; }

        [XmlAttribute("WeaponTwo")]
        public Int32 WeaponTwo { set; get; }

    }

    [Serializable]
    public class DBConfigInfo
    {


        [XmlAttribute("ConnectHost")]
        public String ConnectHost { get; set; }

        [XmlAttribute("Port")]
        public UInt16 Port { get; set; }

        [XmlAttribute("DataBase")]
        public String DataBase { get; set; }

        [XmlAttribute("UserName")]
        public String UserName { get; set; }

        [XmlAttribute("Password")]
        public String Password { get; set; }
    }
    [Serializable]
    public class GameServerContext
    {
        [XmlAttribute("AsyncActionQueueCount")]
        public UInt32 AsyncActionQueueCount { get; set; }

        /// <summary>
        /// 心跳包的发送间隔
        /// </summary>
        [XmlAttribute("HeartBeatTimerPeriod")]
        public int HeartBeatTimerPeriod { get; set; }

    }

    /// <summary>
    /// 游戏匹配配置信息，实例代表一个匹配队列
    /// </summary>
    [Serializable]
    public class GameBarrierConfig
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Level")]
        public int Level { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("MemberCount")]
        public int MemberCount { get; set; }

        [XmlAttribute("Time")]
        public long Time { get; set; }

        [XmlArray("TaskConfigs"), XmlArrayItem("TaskItem")]
        public TaskConfigs[] TaskConfigs { get; set; }

        
    }


    [Serializable]
    public class TaskConfigs
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("StartCondition")]
        public int StartCondition { get; set; }

        [XmlAttribute("Result")]
        public int Result { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("TaskAward")]
        public string TaskAward { get; set; }


        [XmlArray("TaskConditionItemConfig"), XmlArrayItem("Condition")]
        public TaskConditionItemConfig[] TaskConditionItemConfig { get; set; }


    }


    [Serializable]
    public class TaskConditionItemConfig
    {
        [XmlAttribute("ConditionType")]
        public int ConditionType { get; set; }

        [XmlAttribute("ConditionTarget")]
        public int ConditionTarget { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("ConditionValue")]
        public int ConditionValue { get; set; }
    }

    [Serializable]
    public class GameMatchTeamConfig
    {
        //最大队伍数
        [XmlAttribute("TeamMaxCount")]
        public int MaxCount { get; set; }
        //队伍最大容量
        [XmlAttribute("TeamCapacity")]
        public int TeamCapacity { get; set; }
    }
    [Serializable]
    public class GameServerPlayerContext
    {
        /// <summary>
        /// 玩家连接状态下还没有进入认证完成状态的超时时间，时间单位：毫秒
        /// </summary>
        [XmlAttribute("ConnectTimeOut")]
        public Double ConnectTimeOut { get; set; }

        /// <summary>
        /// 玩家断线状态下的超时时间，时间单位：毫秒
        /// </summary>
        [XmlAttribute("DisconnectTimeOut")]
        public Double DisconnectTimeOut { get; set; }

        /// <summary>
        /// sessiontoken的过期时间，时间单位：毫秒
        /// </summary>
        [XmlAttribute("SessionTokenTimeOut")]
        public Double SessionTokenTimeOut { get; set; }
        /// <summary>
        /// AuthToken的过期时间，时间单位：毫秒
        /// </summary>
        [XmlAttribute("AuthTokenTimeOut")]
        public Double AuthTokenTimeOut { get; set; }

        /// <summary>
        /// shutdown过程的超时时间，时间单位：毫秒
        /// </summary>
        [XmlAttribute("ShutdownTimeOut")]
        public Double ShutdownTimeOut { get; set; }
    }

}
