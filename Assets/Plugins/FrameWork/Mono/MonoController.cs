using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Mono控制器 提供操控自己的Update、协程相关功能
public class MonoController : MonoBehaviour
{
    // Update时需调用的事件
    private UnityAction updateAction;
    // FixedUpdate时需调用的事件
    private UnityAction fixedUpdateAction;

    private void Start()
    {
        // 禁止摧毁
        DontDestroyOnLoad(this);
    }

    void Update() => updateAction?.Invoke();

    void FixedUpdate() => fixedUpdateAction?.Invoke();

    // 为Update添加事件
    public void AddUpdateListener(UnityAction action) => updateAction += action;

    // 为Update移除事件
    public void RemoveUpdateListener(UnityAction action) => updateAction -= action;

    // 为FixedUpdate添加事件
    public void AddFixedUpdateListener(UnityAction action) => fixedUpdateAction += action;

    // 为FixedUpdate移除事件
    public void RemoveFixedUpdateListener(UnityAction action) => fixedUpdateAction -= action;
}
