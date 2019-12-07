using Crazy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    public enum DLLType
    {
        Common,
        Client,
        ServerBase,
        GameServer,
    }
    /// <summary>
    /// 用来管理所有打了标签的类型
    /// </summary>
    public class TypeManager
    {
        

        public TypeManager()
        {

        }
        //字典保存所有的类型，key：attribute Type ，value： Type of List
        private readonly UnOrderMultiMap<Type, Type> types = new UnOrderMultiMap<Type, Type>();
        //字典保存所有的程序集的实例，key：程序集枚举类型值 ，value：程序集
        private readonly Dictionary<DLLType, Assembly> assemblies = new Dictionary<DLLType, Assembly>();
        //在服务器启动时将所有的程序集的类型加载到内存中
        public void Add(DLLType dllType, Assembly assembly)
        {

            //将程序集类型加载到字典中
            this.assemblies[dllType] = assembly;
            //清空types
            this.types.Clear();
            //该循环作用是将所有打了特性的类型都加载到types字典中
            foreach (Assembly value in this.assemblies.Values)
            {
                foreach (Type type in value.GetTypes())
                {
                    //找到该类型打了何种特性的标签
                    object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), false);
                    if (objects.Length == 0)
                    {
                        continue;
                    }
                    //因为每个类型我只会打一个特性，所以只取一个
                    BaseAttribute baseAttribute = (BaseAttribute)objects[0];
                    this.types.Add(baseAttribute.AttributeType, type);
                }
            }         
        }
        /// <summary>
        /// 获取所有的类型
        /// </summary>
        /// <param name="systemAttributeType">Attribute Type</param>
        /// <returns></returns>
        public List<Type> GetTypes(Type systemAttributeType)
        {
            if (!this.types.ContainsKey(systemAttributeType))
            {
                return new List<Type>();
            }
            return this.types[systemAttributeType];
        }


        private static TypeManager _instance;
        public static TypeManager Instance
        {
            protected set
            {

            }
            get
            {
                if (_instance == null)
                {
                    _instance = new TypeManager();
                    return _instance;
                }
                return _instance;
            }
        }
    }
}
