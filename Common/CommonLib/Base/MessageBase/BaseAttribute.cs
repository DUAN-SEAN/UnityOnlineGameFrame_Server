using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    /// <summary>
    /// 定义特性基类型，所有特性类都继承于此
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseAttribute : Attribute
    {
        public Type AttributeType { get; }
        /// <summary>
        /// 无参构造器，保存特性的Type
        /// </summary>
        public BaseAttribute()
        {
            this.AttributeType = this.GetType();//获得attribute的实际类型
        }
    }
}
