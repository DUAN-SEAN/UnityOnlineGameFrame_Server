using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    interface ITankPhsical
    {
        
    }



    interface ITankContainer:ITankPhsical
    {
        int GetHealth();
    }
    /// <summary>
    /// 坦克对内接口
    /// </summary>
    interface IInternalTankContainer : ITankContainer
    {
        
    }
}
