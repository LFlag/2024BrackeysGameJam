using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 输入轴类型
/// </summary>
public enum E_InputAxis
{
    Horizontal, // 水平轴按键
    Vertical, // 竖直轴按键
    Mouse_ScrollWheel, // 鼠标滚轮轴
    Mouse_X, // 鼠标水平轴
    Mouse_Y, // 鼠标竖直轴
}

/// <summary>
/// 检测输入的时间类型
/// </summary>
public enum E_CheckTime
{
    Update, // 帧更新时检测
    FixedUpdate, // 物理更新时检测
}

// 输入管理器 提供外部检测按键按下、抬起功能
public class InputMgr : BaseManager<InputMgr>
{
    // 输入检测是否打开
    private bool isOpen = false;
    // 输入检测事件
    private UnityAction updateCheck;
    private UnityAction fixedUpdateCheck;

    // 使用Mono管理器的Update、FixedUpdate来进行输入检测
    public InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
        MonoMgr.Instance.AddFixedUpdateListener(FixedUpdate);
    }

    /// <summary>
    /// 设置输入检测状态
    /// </summary>
    /// <param name="state"> 状态 </param>
    public void SetInputState(bool state) => isOpen = state;

    /// <summary>
    /// 检测当前是否有任意键处于按下状态
    /// </summary>
    private void CheckAnyKey()
    {
        if (Input.anyKey)
            EventCenter.Instance.BroadCastEvent(E_EventType.AnyKey);
    }

    /// <summary>
    /// 检测任意键按下
    /// </summary>
    private void CheckAnyKeyDown()
    {
        if (Input.anyKeyDown)
            EventCenter.Instance.BroadCastEvent(E_EventType.AnyKeyDown);
    }

    /// <summary>
    /// 检测按键按下
    /// </summary>
    /// <param name="key"> 按键值 </param>
    private void CheckKeyDown(KeyCode key)
    {
        if (Input.GetKeyDown(key))
            // 若按下对应按键 事件中心则广播按键按下事件
            EventCenter.Instance.BroadCastEvent<KeyCode>(E_EventType.KeyDown, key);
    }

    /// <summary>
    /// 检测按键抬起
    /// </summary>
    /// <param name="key"> 按键值 </param>
    private void CheckKeyUp(KeyCode key)
    {
        if (Input.GetKeyUp(key))
            // 若抬起对应按键 事件中心则广播按键抬起事件
            EventCenter.Instance.BroadCastEvent<KeyCode>(E_EventType.KeyUp, key);
    }

    /// <summary>
    /// 检测按键持续按下
    /// </summary>
    /// <param name="key"> 按键值 </param>
    private void CheckKeyStay(KeyCode key)
    {
        if (Input.GetKey(key))
            // 若抬起对应按键 事件中心则广播按键抬起事件
            EventCenter.Instance.BroadCastEvent<KeyCode>(E_EventType.KeyStay, key);
    }

    /// <summary>
    /// 检测轴按下
    /// </summary>
    /// <param name="name"> 轴名 </param>
    private void GetAxis(E_InputAxis axis)
    {
        // 若按下对应轴键 事件中心则广播轴按下事件 并返回对应的值
        switch (axis)
        {
            case E_InputAxis.Horizontal:
                EventCenter.Instance.BroadCastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Horizontal"));
                break;
            case E_InputAxis.Vertical:
                EventCenter.Instance.BroadCastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Vertical"));
                break;
            case E_InputAxis.Mouse_ScrollWheel:
                EventCenter.Instance.BroadCastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Mouse ScrollWheel"));
                break;
            case E_InputAxis.Mouse_X:
                EventCenter.Instance.BroadCastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Mouse X"));
                break;
            case E_InputAxis.Mouse_Y:
                EventCenter.Instance.BroadCastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Mouse Y"));
                break;
        }
    }

    /// <summary>
    /// 添加检测当前任意键是否处于按下的监听
    /// </summary>
    public void AddAnyKeyListener() => updateCheck += CheckAnyKey;

    /// <summary>
    /// 添加检测任意键按下的监听
    /// </summary>
    public void AddAnyKeyDownListener() => updateCheck += CheckAnyKeyDown;

    /// <summary>
    // 添加检测按键按下的监听
    /// </summary>
    /// <param name="key"> 按键值 </param>
    /// <param name="checkTime"> 检测时间 </param>
    public void AddKeyDownListener(KeyCode key, E_CheckTime checkTime = E_CheckTime.Update)
    {
        switch (checkTime)
        {
            case E_CheckTime.Update:
                updateCheck += () => CheckKeyDown(key);
                break;
            case E_CheckTime.FixedUpdate:
                fixedUpdateCheck += () => CheckKeyDown(key);
                break;
        }
    }

    /// <summary>
    /// 添加检测按键按下的监听
    /// </summary>
    /// <param name="key"> 按键值 </param>
    /// /// <param name="checkTime"> 检测时间 </param>
    public void AddKeyUpListener(KeyCode key, E_CheckTime checkTime = E_CheckTime.Update)
    {
        switch (checkTime)
        {
            case E_CheckTime.Update:
                updateCheck += () => CheckKeyUp(key);
                break;
            case E_CheckTime.FixedUpdate:
                fixedUpdateCheck += () => CheckKeyUp(key);
                break;
        }
    }

    /// <summary>
    /// 添加检测按键按下的监听
    /// </summary>
    /// <param name="key"> 按键值 </param>
    /// /// <param name="checkTime"> 检测时间 </param>
    public void AddKeyStayListener(KeyCode key, E_CheckTime checkTime = E_CheckTime.Update)
    {
        switch (checkTime)
        {
            case E_CheckTime.Update:
                updateCheck += () => CheckKeyStay(key);
                break;
            case E_CheckTime.FixedUpdate:
                fixedUpdateCheck += () => CheckKeyStay(key);
                break;
        }
    }

    /// <summary>
    /// 添加检测轴按下的监听
    /// </summary>
    /// <param name="name"> 按键值 </param>
    /// /// <param name="checkTime"> 检测时间 </param>
    public void AddGetAxisListener(E_InputAxis axis, E_CheckTime checkTime = E_CheckTime.Update)
    {
        switch (checkTime)
        {
            case E_CheckTime.Update:
                updateCheck += () => GetAxis(axis);
                break;
            case E_CheckTime.FixedUpdate:
                fixedUpdateCheck += () => GetAxis(axis);
                break;
        }
    }

    /// <summary>
    /// 清空输入检测事件
    /// </summary>
    public void ClearInputCheck()
    {
        updateCheck = null;
        fixedUpdateCheck = null;
    }

    // 输入管理的(伪)Update
    private void Update()
    {
        // 如果输入检测关闭 则直接返回
        if (!isOpen) return;
        // 否则 执行存在的输入检测
        updateCheck?.Invoke();
    }

    // 输入管理的(伪)FixedUpdate
    private void FixedUpdate()
    {
        // 如果输入检测关闭 则直接返回
        if (!isOpen) return;
        // 否则 执行存在的输入检测
        fixedUpdateCheck?.Invoke();
    }
}
