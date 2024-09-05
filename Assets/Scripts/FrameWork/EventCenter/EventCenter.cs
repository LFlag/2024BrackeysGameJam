using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

// 采用里氏替换原则 用来存储事件的 空接口
public interface IEventInfo { }

// 两个泛型参数事件
public class EventInfo<T, K> : IEventInfo
{
    public UnityAction<T, K> actions;

    public EventInfo(UnityAction<T, K> action)
    {
        actions += action;
    }
}

// 一个泛型参数事件
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

// 无参事件
public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

// 事件中心
public class EventCenter : BaseManager<EventCenter>
{
    // 选用字典为事件容器 key:事件类型 --> value:关联的事件
    private Dictionary<E_EventType, IEventInfo> eventDic = new Dictionary<E_EventType, IEventInfo>();

    public void AddEventListener<T, K>(E_EventType e_Event, UnityAction<T, K> action)
    {
        // 如果已存在该事件类型 直接添加进对应的事件中
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo<T, K>).actions += action;
        // 否则添加该事件类型和函数
        else
            eventDic.Add(e_Event, new EventInfo<T, K>(action));
    }

    /// <summary>
    /// 为该事件添加监听函数
    /// </summary>
    /// <typeparam name="T"> 参数类型 </typeparam>
    /// <param name="e_Event"> 要监听的事件类型 </param>
    /// <param name="action"> 要添加的监听函数 </param>
    public void AddEventListener<T>(E_EventType e_Event, UnityAction<T> action)
    {
        // 如果已存在该事件类型 直接添加进对应的事件中
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo<T>).actions += action;
        // 否则添加该事件类型和函数
        else
            eventDic.Add(e_Event, new EventInfo<T>(action));
    }

    // 为该事件添加监听函数 无参版
    public void AddEventListener(E_EventType e_Event, UnityAction action)
    {
        // 如果已存在该事件类型 直接添加进对应的事件中
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo).actions += action;
        // 否则添加该事件类型和函数
        else
            eventDic.Add(e_Event, new EventInfo(action));
    }


    /// <summary>
    /// 移除该事件的特定监听函数
    /// </summary>
    /// <typeparam name="T"> 参数类型 </typeparam>
    /// <param name="e_Event"> 要监听的事件类型 </param>
    /// <param name="action"> 要移除的监听函数 </param>
    public void RemoveEventListener<T>(E_EventType e_Event, UnityAction<T> action)
    {
        // 如果存在该类事件类型 则移除对应的监听函数
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo<T>).actions -= action;
    }

    // 为该事件移除监听函数 无参版
    public void RemoveEventListener(E_EventType e_Event, UnityAction action)
    {
        // 如果存在该类事件类型 则移除对应的监听函数
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo).actions -= action;
    }

    public void BroadCastEvent<T, K>(E_EventType e_Event, T info, K value)
    {
        // 如果存在该类事件类型 调用对应的监听函数
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo<T, K>).actions?.Invoke(info, value);
    }

    /// <summary>
    /// 广播该事件 并调用对应的监听函数
    /// </summary>
    /// <typeparam name="T"> 参数类型 </typeparam>
    /// <param name="e_Event"> 事件类型 </param>
    /// <param name="info"> 参数 </param>
    public void BroadCastEvent<T>(E_EventType e_Event, T info)
    {
        // 如果存在该类事件类型 调用对应的监听函数
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo<T>).actions?.Invoke(info);
    }

    // 广播该事件 并调用对应的监听函数 无参版
    public void BroadCastEvent(E_EventType e_Event)
    {
        // 如果存在该类事件类型 调用对应的监听函数
        if (eventDic.ContainsKey(e_Event))
            (eventDic[e_Event] as EventInfo).actions?.Invoke();
    }

    // 清空事件中心
    public void Clear() => eventDic.Clear();
}
