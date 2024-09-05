using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// 场景切换管理器 在基础的场景切换上封装了一层 加载完执行回调函数 的功能
public class SceneMgr : BaseManager<SceneMgr>
{
    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="sceneName"> 场景名称 </param>
    /// <param name="callBack"> 场景加载完毕要执行的回调函数 </param>
    public void LoadScene(string sceneName, UnityAction callBack = null)
    {
        SceneManager.LoadScene(sceneName);
        callBack?.Invoke();
    }

    // 同步加载场景
    public void LoadScene(int sceneBuildIndex, UnityAction callBack = null)
    {
        SceneManager.LoadScene(sceneBuildIndex);
        callBack?.Invoke();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"> 场景名称 </param>
    /// <param name="callBack"> 场景加载完毕要执行的回调函数 </param>
    public void LoadSceneAsync(string sceneName, UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(LoadSceneCoroutine(sceneName, callBack));
    }

    // 异步加载场景
    public void LoadSceneAsync(int sceneBuildIndex, UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(LoadSceneCoroutine(sceneBuildIndex, callBack));
    }

    // 配合异步加载的协程
    IEnumerator LoadSceneCoroutine(string sceneName, UnityAction callBack = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
        {
            // 事件中心 广播场景加载事件 并传递加载进度
            EventCenter.Instance.BroadCastEvent(E_EventType.SceneLoad, ao.progress);
            yield return ao.progress;
        }
        // 场景加载完毕后调用回调函数
        callBack?.Invoke();
    }

    // 配合异步加载的协程
    IEnumerator LoadSceneCoroutine(int sceneBuildIndex, UnityAction callBack = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex);
        while (!ao.isDone)
        {
            // 事件中心 广播场景加载事件 并传递加载进度
            EventCenter.Instance.BroadCastEvent(E_EventType.SceneLoad, ao.progress);
            yield return ao.progress;
        }
        // 场景加载完毕后调用回调函数
        callBack?.Invoke();
    }
}
