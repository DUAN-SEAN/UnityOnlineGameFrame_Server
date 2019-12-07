using Crazy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    public static class  MessageFactory
    {
        public static bool Adapting(ObjectPool objectPool)
        {
            m_objectPool = objectPool;
            if (m_objectPool == null) return false;
            return true;
        }

        public static T CreateMessage<T>() where T : class,IMessage
        {
            if(m_objectPool==null) return (T)Activator.CreateInstance(typeof(T));
            T message = m_objectPool.Fetch<T>();
            
            return message;
        }
        public static object CreateMessage(Type type)
        {
            if (m_objectPool == null) return Activator.CreateInstance(type);
            object message = m_objectPool.Fetch(type);
            return message;
        }
        public static void Recycle(IMessage message)
        {
            if (m_objectPool == null) return;

            m_objectPool.Recycle(message);
        } 

        private static ObjectPool m_objectPool;

    }
}
