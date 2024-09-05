using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// UI层级
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}

public class UIMgr : BaseManager<UIMgr>
{
    // 存储当前所出现的面板
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    // Canvas节点
    public RectTransform canvas;
    // 四层节点
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIMgr()
    {
        // 创建Canvas
        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        // 并且标记为不可摧毁
        GameObject.DontDestroyOnLoad(obj);

        // 分别找到四层节点
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
    }

    /// <summary>
    /// 获取对应层级的节点
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetLayerNode(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;
            case E_UI_Layer.Top:
                return top;
            case E_UI_Layer.System:
                return system;
            default:
                return null;
        }
    }

    // 显示面板
    public T ShowPanel<T>(E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T : BasePanel
    {
        // 得到该面板类的名字（规定和类名一致 方便操作）
        string panelName = typeof(T).Name;
        T panel = null;

        // 如果已有 则直接返回
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        else
        {
            // 否则 动态创建面板
            ResMgr.Instance.LoadAsync<GameObject>("UI/Panel/" + panelName, (obj) =>
            {
                obj.transform.SetParent(GetLayerNode(layer), false);
                // 得到面板类
                panel = obj.GetComponent<T>();
                // 执行回调函数
                callBack?.Invoke(panel);
                // 显示面板
                panel.ShowMe();
                // 添加进面板字典中
                panelDic.Add(panelName, panel);
            });
        }

        return panel;
    }

    // 关闭面板
    public void HidePanel<T>() where T : BasePanel
    {
        // 得到该面板类的名字（规定和类名一致 方便操作）
        string panelName = typeof(T).Name;

        // 如果当前有该面板
        if (panelDic.ContainsKey(panelName))
        {
            T panel = panelDic[panelName] as T;
            // 关闭面板
            panel.HideMe(() =>
            {
                // 销毁面板对象
                GameObject.Destroy(panel.gameObject);
                // 从字典中移除
                panelDic.Remove(panelName);
            });
        }
    }

    // 获取面板
    public T GetPanel<T>() where T : BasePanel
    {
        // 得到该面板类的名字（规定和类名一致 方便操作）
        string panelName = typeof(T).Name;

        // 如果当前有该面板 则返回
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //否则返回 null
        return null;
    }

    /// <summary>
    /// 为UI控件添加自定义监听事件
    /// </summary>
    /// <param name="control"> UI控件 </param>
    /// <param name="type"> 事件类型 </param>
    /// <param name="callBack"> 事件的监听事件 </param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        // 获取EventTrigger 如果没有就添加
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        // 创建Entry并设置对应的事件类型、添加监听事件
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        // 将Entry加入Trigger
        trigger.triggers.Add(entry);
    }
}
