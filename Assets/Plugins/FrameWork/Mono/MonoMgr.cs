using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

// Mono管理器 利用Mono控制器 为没继承Mono的类提供使用Update、协程相关功能
public class MonoMgr : BaseManager<MonoMgr>
{
    // 管理的Mono控制器
    private MonoController controller;

    // 保证Mono控制器的唯一性
    public MonoMgr()
    {
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }

    #region 封装一层添加FixedUpdate添加事件、移除事件的功能
    public void AddFixedUpdateListener(UnityAction action) => controller.AddFixedUpdateListener(action);

    public void RemoveFixedUpdateListener(UnityAction action) => controller.RemoveFixedUpdateListener(action);
    #endregion

    #region 封装一层添加Update添加事件、移除事件的功能
    public void AddUpdateListener(UnityAction action) => controller.AddUpdateListener(action);

    public void RemoveUpdateListener(UnityAction action) => controller.RemoveUpdateListener(action);
    #endregion


    #region 封装一层使用协程相关的功能
    public Coroutine StartCoroutine(string methodName) => controller.StartCoroutine(methodName);

    public Coroutine StartCoroutine(IEnumerator routine) => controller.StartCoroutine(routine);

    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value) => controller.StartCoroutine(methodName, value);

    public void StopCoroutine(IEnumerator routine) => controller.StopCoroutine(routine);

    public void StopCoroutine(Coroutine routine) => controller.StopCoroutine(routine);

    public void StopCoroutine(string methodName) => controller.StopCoroutine(methodName);

    public void StopAllCoroutines() => controller.StopAllCoroutines();
    #endregion
}
