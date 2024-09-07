using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework
{
    /// <summary>
    /// UI层级
    /// </summary>
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
        private Dictionary<string, BasePanel> _panelDic = new Dictionary<string, BasePanel>();

        // Canvas节点
        private RectTransform Canvas;

        // 四层节点
        private Transform _bot;
        private Transform _mid;
        private Transform _top;
        private Transform _system;

        public UIMgr()
        {
            // 创建Canvas
            var obj = ResMgr.Instance.Load<GameObject>("UI/Canvas");
            Canvas = obj.transform as RectTransform;
            // 并且标记为不可摧毁
            GameObject.DontDestroyOnLoad(obj);

            // 分别找到四层节点
            _bot = Canvas!.Find("Bot");
            _mid = Canvas.Find("Mid");
            _top = Canvas.Find("Top");
            _system = Canvas.Find("System");
        }

        /// <summary>
        /// 获取对应层级的节点
        /// </summary>
        /// <param name="layer"> 层级类型 </param>
        /// <returns> 层级节点 </returns>
        public Transform GetLayerNode(E_UI_Layer layer)
        {
            switch (layer)
            {
                case E_UI_Layer.Bot:
                    return _bot;
                case E_UI_Layer.Mid:
                    return _mid;
                case E_UI_Layer.Top:
                    return _top;
                case E_UI_Layer.System:
                    return _system;
                default:
                    return null;
            }
        }

        // 显示面板
        public T ShowPanel<T>(E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T : BasePanel
        {
            // 得到该面板类的名字（规定和类名一致 方便操作）
            var panelName = typeof(T).Name;
            T panel = null;

            // 如果已有 则直接返回
            if (_panelDic.ContainsKey(panelName))
            {
                panel = _panelDic[panelName] as T;
            }
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
                    _panelDic.Add(panelName, panel);
                });
            }

            return panel;
        }

        // 关闭面板
        public void HidePanel<T>() where T : BasePanel
        {
            // 得到该面板类的名字（规定和类名一致 方便操作）
            var panelName = typeof(T).Name;

            // 如果当前有该面板
            if (_panelDic.ContainsKey(panelName))
            {
                var panel = _panelDic[panelName] as T;
                // 关闭面板
                panel!.HideMe(() =>
                {
                    // 销毁面板对象
                    GameObject.Destroy(panel.gameObject);
                    // 从字典中移除
                    _panelDic.Remove(panelName);
                });
            }
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        /// <typeparam name="T"> 面板类型 </typeparam>
        /// <returns> 面板 </returns>
        public T GetPanel<T>() where T : BasePanel
        {
            // 得到该面板类的名字（规定和类名一致 方便操作）
            var panelName = typeof(T).Name;
            
            if (_panelDic.ContainsKey(panelName))
                return _panelDic[panelName] as T;
            
            return null;
        }

        /// <summary>
        /// 为UI控件添加自定义监听事件
        /// </summary>
        /// <param name="control"> UI控件 </param>
        /// <param name="type"> 事件类型 </param>
        /// <param name="callBack"> 事件的监听事件 </param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,
            UnityAction<BaseEventData> callBack)
        {
            // 获取EventTrigger 如果没有就添加
            var trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = control.gameObject.AddComponent<EventTrigger>();

            // 创建Entry并设置对应的事件类型、添加监听事件
            var entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callBack);

            // 将Entry加入Trigger
            trigger.triggers.Add(entry);
        }
    }
}