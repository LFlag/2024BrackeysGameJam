using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 继承Mono的自动生成的单例基类
    /// </summary>
    /// <typeparam name="T"> 单例类型 </typeparam>
    public class BaseManagerAutoMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    var _self = new GameObject();
                    _self.name = typeof(T).Name;
                    instance = _self as T;
                }

                return instance;
            }
        }
    }
}
