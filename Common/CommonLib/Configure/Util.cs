using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Crazy.Common
{

    public class XmlAttributeOverridesItem
    {
        public XmlAttributeOverridesItem(Type ownerType, String xmlFieldName, String fieldName, Type replacementType)
        {
            OwnerType = ownerType;
            XmlFieldName = xmlFieldName;
            FieldName = fieldName;
            ReplacementType = replacementType;
        }

        public Type OwnerType { get; private set; }
        public String XmlFieldName { get; private set; }
        public String FieldName { get; private set; }
        public Type ReplacementType { get; private set; }
    }

    /// <summary>
    /// Configuration工具类，提供辅助函数。
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 反序列化Xml文件到指定的类型对象。
        /// </summary>
        /// <typeparam name="T">反虚拟化的对象类型。</typeparam>
        /// <param name="xmlPath">Xml文件的路径。</param>
		/// <param name="attrOverrrides"></param>
        /// <returns>反虚拟化的对象。抛出异常或者返回null都表示失败。</returns>
        public static T Deserialize<T>(String xmlPath, XmlAttributeOverrides attrOverrides = null) where T : class
        {
            T tOut;

            /// 使用System.Xml.Serialization.XmlSerializer反序列化Xml文件
			XmlSerializer xmlSeliz = new XmlSerializer(typeof(T), attrOverrides ?? new XmlAttributeOverrides());
            /// 确保FileStream最终能够释放
            using (Stream xmlRd = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
            {
                var xmlInfo = File.ReadAllText(xmlPath);
                Console.WriteLine($"{xmlInfo}");

                tOut = xmlSeliz.Deserialize(xmlRd) as T;
            }

            return tOut;
        }

        /// <summary>
        /// 序列化指定的类型对象到Xml文件。
        /// </summary>
        /// <typeparam name="T">虚拟化的对象类型。</typeparam>
        /// <param name="xmlPath">Xml文件的路径。</param>
		/// <param name="attrOverrrides"></param>
        /// <remark>抛出异常表示失败。</remark>
		public static void Serialize<T>(T dataObj, String xmlPath, XmlAttributeOverrides attrOverrides = null) where T : class
        {
            /// 使用System.Xml.Serialization.XmlSerializer序列化Xml文件
			XmlSerializer xmlSeliz = new XmlSerializer(typeof(T), attrOverrides ?? new XmlAttributeOverrides());
            /// 确保FileStream最终能够释放
            using (Stream xmlWr = new FileStream(xmlPath, FileMode.Create))
            {
                xmlSeliz.Serialize(xmlWr, dataObj);
            }
        }

        public static void ReplaceToDerived(this XmlAttributeOverrides overrides, Type ownerType,
                                            String xmlFieldName, String fieldName, Type replacementType)
        {
            // xmlFieldName字段当做replacementType来序列化
            XmlElementAttribute xElement = new XmlElementAttribute(xmlFieldName, replacementType);

            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlElements.Add(xElement);

            // 这里第二个参数的字符串，应该是ownerType里字段原始名称，而不是XmlElement里定义的xmlFieldName名称。
            overrides.Add(ownerType, fieldName, attrs);
        }
    }
}
