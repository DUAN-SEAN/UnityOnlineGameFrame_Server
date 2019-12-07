using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    /// <summary>
    /// 该类型用于加载程序集
    /// 在运行期间调用，首先第一个作用就是将所有定义的走网络消息的message读取出
    /// 
    /// </summary>
    public class AssemblyHelper
    {
        /// <summary>
        /// 获取程序集中的所有类型
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public List<Type> Load(Assembly assembly)
        {
            return assembly.GetTypes().ToList<Type>();
        }
    }
}
