using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.NetSharp;
using Crazy.Common;
using System.IO;
using System.Threading;

namespace Crazy.ServerBase
{
    /// <summary>
    /// 玩家现场基类
    /// 目前玩家现场的逻辑由Client的任务链管理，同一个玩家的逻辑保证先后执行的顺序。
    /// 只要调用玩家的PostLocalMessage即可，在Onmessage中写逻辑代码。
    /// </summary>
    public class ServerContextBase: PlayerContextBase
    {
        public ServerContextBase(int id, ServerType serverType)
        {
            this.m_serverId = id;
            this.m_serverType = serverType;
        }
      
        public int ServerId => m_serverId;

        public ServerType ServerType => m_serverType;

        protected int m_serverId;

        protected ServerType m_serverType;
    }
}
