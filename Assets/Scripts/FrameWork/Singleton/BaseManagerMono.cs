using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承Mono的自动生成的单例基类
public class BaseManagerMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject _self = new GameObject();
                _self.name = typeof(T).Name;
                instance = _self as T;
            }
            return instance;
        }
    }
}
