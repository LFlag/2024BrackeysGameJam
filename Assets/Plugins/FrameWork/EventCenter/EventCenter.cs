using System.Collections.Generic;
using UnityEngine.Events;

// 事件中心管理的事件类型
public enum E_EventType
{
    SceneLoad, // 场景加载
    AnyKey, // 任意键处于按下的状态
    AnyKeyDown, // 任意键按下
    KeyDown, // 按键按下
    KeyUp, // 按键抬起
    KeyStay, // 按键持续被按下
    GetAxis, // 轴按下
}

#region 事件的数据结构

// 采用里氏替换原则 用来存储事件的 空接口
public interface IEventInfo { }

// 两个泛型参数事件
public class EventInfo<T1, T2> : IEventInfo
{
    public UnityAction<T1, T2> Actions;

    public EventInfo(UnityAction<T1, T2> action) => Actions += action;
}

// 一个泛型参数事件
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> Actions;

    public EventInfo(UnityAction<T> action) => Actions += action;
}

// 无参事件
public class EventInfo : IEventInfo
{
    public UnityAction Actions;

    public EventInfo(UnityAction action) => Actions += action;
}

#endregion

// 事件中心
public class EventCenter : BaseManager<EventCenter>
{
    // 选用字典为事件容器 key:事件类型 --> value:关联的事件
    private Dictionary<E_EventType, IEventInfo> _eventDic = new Dictionary<E_EventType, IEventInfo>();

    #region AddEventListener
    
    /// <summary>
    /// 添加事件的监听函数
    /// </summary>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="action"> 监听函数 </param>
    /// <typeparam name="T1"> 参数1类型 </typeparam>
    /// <typeparam name="T2"> 参数2类型 </typeparam>
    public void AddEventListener<T1, T2>(E_EventType eventType, UnityAction<T1, T2> action)
    {
        // 如果已存在该事件类型 直接添加进对应的事件中
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo<T1, T2>)!.Actions += action;
        // 否则添加该事件类型和函数
        else
            _eventDic.Add(eventType, new EventInfo<T1, T2>(action));
    }

    /// <summary>
    /// 添加事件的监听函数
    /// </summary>
    /// <typeparam name="T"> 参数类型 </typeparam>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="action"> 监听函数 </param>
    public void AddEventListener<T>(E_EventType eventType, UnityAction<T> action)
    {
        // 如果已存在该事件类型 直接添加进对应的事件中
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo<T>)!.Actions += action;
        // 否则添加该事件类型和函数
        else
            _eventDic.Add(eventType, new EventInfo<T>(action));
    }

    /// <summary>
    /// 添加事件的监听函数
    /// </summary>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="action"> 监听函数 </param>
    public void AddEventListener(E_EventType eventType, UnityAction action)
    {
        // 如果已存在该事件类型 直接添加进对应的事件中
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo)!.Actions += action;
        // 否则添加该事件类型和函数
        else
            _eventDic.Add(eventType, new EventInfo(action));
    }
    
    #endregion

    #region RemoveListener
    
    /// <summary>
    /// 移除事件的监听函数
    /// </summary>
    /// <typeparam name="T1"> 参数1类型 </typeparam>
    /// <typeparam name="T2"> 参数2类型 </typeparam>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="action"> 监听函数 </param>
    public void RemoveEventListener<T1, T2>(E_EventType eventType, UnityAction<T1, T2> action)
    {
        // 如果存在该类事件类型 则移除对应的监听函数
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo<T1, T2>)!.Actions -= action;
    }
    
    /// <summary>
    /// 移除事件的监听函数
    /// </summary>
    /// <typeparam name="T"> 参数类型 </typeparam>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="action"> 监听函数 </param>
    public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> action)
    {
        // 如果存在该类事件类型 则移除对应的监听函数
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo<T>)!.Actions -= action;
    }
    
    /// <summary>
    /// 移除事件的监听函数
    /// </summary>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="action"> 监听函数 </param>
    public void RemoveEventListener(E_EventType eventType, UnityAction action)
    {
        // 如果存在该类事件类型 则移除对应的监听函数
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo)!.Actions -= action;
    }

    #endregion

    #region BroadcastEvent
    
    /// <summary>
    /// 广播事件
    /// </summary>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="arg1"> 参数1 </param>
    /// <param name="arg2"> 参数2 </param>
    /// <typeparam name="T1"> 参数1类型 </typeparam>
    /// <typeparam name="T2"> 参数2类型 </typeparam>
    public void BroadcastEvent<T1, T2>(E_EventType eventType, T1 arg1, T2 arg2)
    {
        // 如果存在该类事件类型 调用对应的监听函数
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo<T1, T2>)!.Actions?.Invoke(arg1, arg2);
    }

    /// <summary>
    /// 广播事件
    /// </summary>
    /// <typeparam name="T"> 参数类型 </typeparam>
    /// <param name="eventType"> 事件类型 </param>
    /// <param name="arg"> 参数 </param>
    public void BroadcastEvent<T>(E_EventType eventType, T arg)
    {
        // 如果存在该类事件类型 调用对应的监听函数
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo<T>)!.Actions?.Invoke(arg);
    }

    /// <summary>
    /// 广播事件
    /// </summary>
    /// <param name="eventType"> 事件类型 </param>
    public void BroadcastEvent(E_EventType eventType)
    {
        // 如果存在该类事件类型 调用对应的监听函数
        if (_eventDic.TryGetValue(eventType, out var eventInfo))
            (eventInfo as EventInfo)!.Actions?.Invoke();
    }
    
    #endregion

    /// <summary>
    /// 清空事件中心
    /// </summary>
    public void Clear() => _eventDic.Clear();
}
