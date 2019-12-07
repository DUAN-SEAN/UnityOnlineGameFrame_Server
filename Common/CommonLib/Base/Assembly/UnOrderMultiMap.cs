using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crazy.Common { 
    /// <summary>
    /// 类型映射字典集 一对多的映射关系
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class UnOrderMultiMap<T, K>
    {
        private readonly Dictionary<T, List<K>> dictionary = new Dictionary<T, List<K>>();

        // 重用list
        private readonly Queue<List<K>> queue = new Queue<List<K>>();

        public Dictionary<T, List<K>> GetDictionary()
        {
            return this.dictionary;
        }
        /// <summary>
        /// 根据T的类型找到对应保存K类型对象的集合并且将k对象保存到字典中
        /// </summary>
        /// <param name="t"></param>
        /// <param name="k"></param>
        public void Add(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                list = this.FetchList();
                this.dictionary[t] = list;
            }
            list.Add(k);
        }

        public KeyValuePair<T, List<K>> First()
        {
            return this.dictionary.First();
        }
        /// <summary>
        /// 返回字典长度
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        private List<K> FetchList()
        {
            if (this.queue.Count > 0)
            {
                List<K> list = this.queue.Dequeue();
                list.Clear();
                return list;
            }
            return new List<K>();
        }

        private void RecycleList(List<K> list)
        {
            // 防止暴涨
            if (this.queue.Count > 100)
            {
                return;
            }
            list.Clear();
            this.queue.Enqueue(list);
        }
        /// <summary>
        /// 找到t对应的集合，并且删除k
        /// </summary>
        /// <param name="t">字典key</param>
        /// <param name="k">待删k类型实例</param>
        /// <returns></returns>
        public bool Remove(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                this.RecycleList(list);
                this.dictionary.Remove(t);
            }
            return true;
        }

        public bool Remove(T t)
        {
            List<K> list = null;
            this.dictionary.TryGetValue(t, out list);
            if (list != null)
            {
                this.RecycleList(list);
            }
            return this.dictionary.Remove(t);
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return new K[0];
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<K> this[T t]
        {
            get
            {
                List<K> list;
                this.dictionary.TryGetValue(t, out list);
                return list;
            }
        }

        public K GetOne(T t)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default(K);
        }

        public bool Contains(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }

        public bool ContainsKey(T t)
        {
            return this.dictionary.ContainsKey(t);
        }

        public void Clear()
        {
            foreach (KeyValuePair<T, List<K>> keyValuePair in this.dictionary)
            {
                this.RecycleList(keyValuePair.Value);
            }
            this.dictionary.Clear();
        }
    }
}
