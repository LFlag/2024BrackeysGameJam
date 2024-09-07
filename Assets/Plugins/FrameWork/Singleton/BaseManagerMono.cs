using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utility;

namespace Framework
{
    /// <summary>
    /// 继承Mono的需要挂载的单例基类
    /// </summary>
    /// <typeparam name="T"> 单例类型 </typeparam>
    public abstract class BaseManagerMono<T> : MonoBehaviour where T : BaseManagerMono<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();
                if (_instance)
                {
                    _instance.OnInit();
                    return _instance;
                }

                DebugUtil.LogError($"当前场景未挂载{typeof(T).Name}单例脚本");
                return null;
            }
        }

        protected virtual bool IsDontDestroyOnLoad { get; set; } = true;

        private void OnInit()
        {
            transform.SetParent(null);
            if (IsDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            Init();
        }

        protected virtual void Init() { }

        private void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                _instance!.OnInit();
            }
            else
            {
                if (_instance != this) // 如果已存在改单例 且不是自己 则销毁自己
                    Destroy(gameObject);
            }
        }
    }
}
