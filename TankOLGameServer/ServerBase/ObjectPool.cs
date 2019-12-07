using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    
    public class ObjectPool
    {
        private readonly Dictionary<Type, Queue<object>> dictionary = new Dictionary<Type, Queue<object>>();
        /// <summary>
        /// 根据组件类型实例化组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <returns>返回Component类型</returns>
        public object Fetch(Type type)
        {
            Queue<object> queue;
            if (!this.dictionary.TryGetValue(type, out queue))
            {
                queue = new Queue<object>();
                this.dictionary.Add(type, queue);
            }
            object obj;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
            }
            else
            {
                obj = (object)Activator.CreateInstance(type);
            }
            
            return obj;
        }
        /// <summary>
        /// 根据组件类型生成组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>返回该组件类型</returns>
        public T Fetch<T>()
        {
            T t = (T)this.Fetch(typeof(T));
            return t;
        }
        /// <summary>
        /// 将组件放入队列字典中
        /// </summary>
        /// <param name="obj">组件实例</param>
        public void Recycle(object obj)
        {
            Type type = obj.GetType();
            Queue<object> queue;
            if (!this.dictionary.TryGetValue(type, out queue))
            {
                queue = new Queue<object>();
                this.dictionary.Add(type, queue);
            }
            //内存降温
            if (queue.Count > 10)
            {
                queue.Clear();
            }
            queue.Enqueue(obj);
        }
    }
}
