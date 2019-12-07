using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;


namespace Crazy.Common
{
    /// <summary>
    /// 操作全局配置对象。
    /// </summary>
    public class ConfigLoader<T> where T : class
    {
        /// <summary>
        /// 配置信息对象，在调用Initialize后生效。
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// 初始化全局配置。
        /// </summary>
        /// <param name="globalConfPath">全局配置文件的路径。</param>
        public bool Initialize(String globalConfPath)
        {
            try
            {
				XmlAttributeOverrides overrides = TryGetXmlAttributeOverrides();

				/// 使用辅助函数反序列化Configure对象
                Data = Util.Deserialize<T>(globalConfPath, overrides);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

		public static XmlAttributeOverrides TryGetXmlAttributeOverrides()
		{
			PropertyInfo prop;
            Type current = typeof(T);
            while ((prop = current.GetProperty("XmlOverrides")) == null && current.BaseType != typeof(Object))
            {
                current = current.BaseType;
            }
            XmlAttributeOverrides xOverrides = new XmlAttributeOverrides();
            if (prop != null)
            {
                foreach (XmlAttributeOverridesItem item in (IEnumerable<XmlAttributeOverridesItem>)prop.GetValue(null))
                {
                    xOverrides.ReplaceToDerived(item.OwnerType, item.XmlFieldName, item.FieldName, item.ReplacementType);
                }
            }
			return xOverrides;
		}
    }
}
