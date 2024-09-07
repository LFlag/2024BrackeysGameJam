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
/// 检测输入的时机类型
/// </summary>
public enum E_CheckTick
{
    Update, // 帧更新时检测
    FixedUpdate, // 物理更新时检测
}

// 输入管理器 提供外部检测按键按下、抬起功能
public class InputMgr : BaseManager<InputMgr>
{
    // 输入检测是否打开
    private bool _isOpen;
    
    // 输入检测事件
    private UnityAction _updateCheck;
    private UnityAction _fixedUpdateCheck;

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
    public void SetInputState(bool state) => _isOpen = state;

    /// <summary>
    /// 检测当前是否有任意键处于按下状态
    /// </summary>
    private void CheckAnyKey()
    {
        if (Input.anyKey)
            EventCenter.Instance.BroadcastEvent(E_EventType.AnyKey);
    }

    /// <summary>
    /// 检测任意键按下
    /// </summary>
    private void CheckAnyKeyDown()
    {
        if (Input.anyKeyDown)
            EventCenter.Instance.BroadcastEvent(E_EventType.AnyKeyDown);
    }

    /// <summary>
    /// 检测按键按下
    /// </summary>
    /// <param name="key"> 按键 </param>
    private void CheckKeyDown(KeyCode key)
    {
        if (Input.GetKeyDown(key))
            EventCenter.Instance.BroadcastEvent<KeyCode>(E_EventType.KeyDown, key);
    }

    /// <summary>
    /// 检测按键抬起
    /// </summary>
    /// <param name="key"> 按键 </param>
    private void CheckKeyUp(KeyCode key)
    {
        if (Input.GetKeyUp(key))
            EventCenter.Instance.BroadcastEvent<KeyCode>(E_EventType.KeyUp, key);
    }

    /// <summary>
    /// 检测按键持续按下
    /// </summary>
    /// <param name="key"> 按键 </param>
    private void CheckKeyStay(KeyCode key)
    {
        if (Input.GetKey(key))
            EventCenter.Instance.BroadcastEvent<KeyCode>(E_EventType.KeyStay, key);
    }

    /// <summary>
    /// 检测轴按下
    /// </summary>
    /// <param name="axis"> 轴 </param>
    private void GetAxis(E_InputAxis axis)
    {
        // 若按下对应轴键 事件中心则广播轴按下事件 并返回对应的值
        switch (axis)
        {
            case E_InputAxis.Horizontal:
                EventCenter.Instance.BroadcastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Horizontal"));
                break;
            case E_InputAxis.Vertical:
                EventCenter.Instance.BroadcastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Vertical"));
                break;
            case E_InputAxis.Mouse_ScrollWheel:
                EventCenter.Instance.BroadcastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Mouse ScrollWheel"));
                break;
            case E_InputAxis.Mouse_X:
                EventCenter.Instance.BroadcastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Mouse X"));
                break;
            case E_InputAxis.Mouse_Y:
                EventCenter.Instance.BroadcastEvent<E_InputAxis, float>(E_EventType.GetAxis, axis, Input.GetAxis("Mouse Y"));
                break;
        }
    }

    /// <summary>
    /// 添加检测当前任意键是否处于按下的监听
    /// </summary>
    public void AddAnyKeyListener() => _updateCheck += CheckAnyKey;

    /// <summary>
    /// 添加检测任意键按下的监听
    /// </summary>
    public void AddAnyKeyDownListener() => _updateCheck += CheckAnyKeyDown;

    /// <summary>
    /// 添加检测按键按下的监听
    /// </summary>
    /// <param name="key"> 按键值 </param>
    /// <param name="checkTick"> 检测时机 </param>
    public void AddKeyDownListener(KeyCode key, E_CheckTick checkTick = E_CheckTick.Update)
    {
        switch (checkTick)
        {
            case E_CheckTick.Update:
                _updateCheck += () => CheckKeyDown(key);
                break;
            case E_CheckTick.FixedUpdate:
                _fixedUpdateCheck += () => CheckKeyDown(key);
                break;
        }
    }

    /// <summary>
    /// 添加检测按键抬起的监听
    /// </summary>
    /// <param name="key"> 按键值 </param>
    /// /// <param name="checkTick"> 检测时机 </param>
    public void AddKeyUpListener(KeyCode key, E_CheckTick checkTick = E_CheckTick.Update)
    {
        switch (checkTick)
        {
            case E_CheckTick.Update:
                _updateCheck += () => CheckKeyUp(key);
                break;
            case E_CheckTick.FixedUpdate:
                _fixedUpdateCheck += () => CheckKeyUp(key);
                break;
        }
    }

    /// <summary>
    /// 添加检测按键持续按下的监听
    /// </summary>
    /// <param name="key"> 按键值 </param>
    /// /// <param name="checkTick"> 检测时机 </param>
    public void AddKeyStayListener(KeyCode key, E_CheckTick checkTick = E_CheckTick.Update)
    {
        switch (checkTick)
        {
            case E_CheckTick.Update:
                _updateCheck += () => CheckKeyStay(key);
                break;
            case E_CheckTick.FixedUpdate:
                _fixedUpdateCheck += () => CheckKeyStay(key);
                break;
        }
    }

    /// <summary>
    /// 添加检测轴按下的监听
    /// </summary>
    /// <param name="axis"> 轴 </param>
    /// /// <param name="checkTick"> 检测时机 </param>
    public void AddGetAxisListener(E_InputAxis axis, E_CheckTick checkTick = E_CheckTick.Update)
    {
        switch (checkTick)
        {
            case E_CheckTick.Update:
                _updateCheck += () => GetAxis(axis);
                break;
            case E_CheckTick.FixedUpdate:
                _fixedUpdateCheck += () => GetAxis(axis);
                break;
        }
    }

    /// <summary>
    /// 清空输入检测事件
    /// </summary>
    public void ClearInputCheck()
    {
        _updateCheck = null;
        _fixedUpdateCheck = null;
    }

    // 输入管理的(伪)Update
    private void Update()
    {
        // 如果输入检测关闭 则直接返回
        if (!_isOpen) 
            return;
        
        // 否则 执行存在的输入检测
        _updateCheck?.Invoke();
    }

    // 输入管理的(伪)FixedUpdate
    private void FixedUpdate()
    {
        // 如果输入检测关闭 则直接返回
        if (!_isOpen) 
            return;
        
        // 否则 执行存在的输入检测
        _fixedUpdateCheck?.Invoke();
    }
}
