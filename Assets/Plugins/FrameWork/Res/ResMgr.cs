using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 资源加载管理器 在Resources资源加载上封装一层 加载完调用回调函数 的功能
    /// </summary>
    public class ResMgr : BaseManager<ResMgr>
    {
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path"> 资源路径 </param>
        /// <param name="callBack"> 加载完调用的回调函数 </param>
        /// <typeparam name="T"> 加载类型 </typeparam>
        /// <returns></returns>
        public T Load<T>(string path, UnityAction<T> callBack = null) where T : Object
        {
            var obj = Resources.Load<T>(path);

            callBack?.Invoke(obj);

            // 如果资源是GameObject 则直接实例化
            return obj is GameObject ? GameObject.Instantiate(obj) : obj;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"> 资源路径 </param>
        /// <param name="callBack"> 加载完调用的回调函数 </param>
        /// <typeparam name="T"> 加载类型 </typeparam>
        public void LoadAsync<T>(string path, UnityAction<T> callBack = null) where T : Object =>
            MonoMgr.Instance.StartCoroutine(LoadCoroutine(path, callBack));

        // 配合异步加载使用的协程
        private IEnumerator LoadCoroutine<T>(string path, UnityAction<T> callBack = null) where T : Object
        {
            var request = Resources.LoadAsync<T>(path);
            yield return request;

            if (request.asset is GameObject)
                callBack?.Invoke(GameObject.Instantiate(request.asset) as T);
            else
                callBack?.Invoke(request.asset as T);
        }
    }
}
