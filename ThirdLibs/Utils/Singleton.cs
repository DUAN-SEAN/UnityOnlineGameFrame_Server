namespace BlackJack.Utils
{
    /// <summary>
    /// 单件模型的实现类。
    /// </summary>
    /// <typeparam name="T">实际要创建的单件对象类型。</typeparam>
    public class Singleton<T> where T : new()
    {
        /// <summary>
        /// 获得单件对象的实例。
        /// </summary>
        public static T Default
        {
            get
            {
                if (m_instance == null)
                    m_instance = new T();
                return m_instance;
            }
        }

        /// <summary>
        /// 单件实例对象。
        /// </summary>
        private static T m_instance;
    }
}
