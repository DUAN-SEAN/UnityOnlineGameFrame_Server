using log4net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ice.Utils
{
    /// <summary>
    /// 我们Sharp同一写日志的类，需要的用
    /// </summary>
    public class SharpLog
    {
        /// <summary>
        /// 静态构造
        /// </summary>
        static SharpLog()
        {
            Default = new SharpLog();

            Default.Log = LogManager.GetLogger(typeof(SharpLog).ToString());
        }

        /// <summary>
        /// 私有构造
        /// </summary>
        private SharpLog() { }

        /// <summary>
        /// 静态实例属性
        /// </summary>
        public static SharpLog Default { get; private set; }

        /// <summary>
        /// Log接口
        /// </summary>
        public ILog Log { get; set; }
    }
}
