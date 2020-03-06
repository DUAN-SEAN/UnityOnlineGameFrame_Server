using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.ServerBase
{
    /// <summary>
    /// 状态机基类，用于服务器上层实体绑定对应的状态机
    /// </summary>
    public abstract class StateMachine
    {
        /// <summary>
        /// 设置新状态，并检查当前状态下是否接受commingEvent，并能够跳转到newState
        /// </summary>
        /// <param name="commingEvent"></param>
        /// <param name="newState">为-1时由状态机自身决定跳转状态，否则将会检查新状态的指定是否是合法的</param>
        /// <param name="testOnly"></param>
        /// <returns>返回-1说明跳转失败，否则返回状态机的当前状态</returns>
        public virtual Int32 SetStateCheck(Int32 commingEvent, Int32 newState = -1)
        {
            State = newState;
            return State;
        }
        /// <summary>
        /// 默认构造 无需检测
        /// </summary>
        /// <param name="newState"></param>
        public virtual void SetStateWithoutCheck(Int32 newState)
        {
            State = newState;
        }

        public virtual bool EventCheck(Int32 commingEvent)
        {
            return (SetStateCheck(commingEvent, -1) != -1);
        }

        public Int32 State { get; protected set; }

        
    }
}
