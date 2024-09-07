using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 基于类型的事件系统
    /// </summary>
    public class TypeEventSystem
    {
        private readonly EventContainer _eventContainer = new EventContainer();

        public IUnRegister Register<T>(Action<T> onEvent) => _eventContainer.GetOrAdd<Event<T>>().Register(onEvent);

        public void UnRegister<T>(Action<T> onEvent) => _eventContainer.Get<Event<T>>().UnRegister(onEvent);

        public void Broadcast<T>(T eventType) => _eventContainer.Get<Event<T>>().Trigger(eventType);
    }

    #region Event

    public interface IUnRegister
    {
        void UnRegister();
    }

    public interface IEvent { }

    public class Event<T> : IEvent
    {
        private Action<T> _onEvent;
        
        public IUnRegister Register(Action<T> onEvent)
        {
            _onEvent += onEvent;
            return new EventUnRegister(() => UnRegister(onEvent));
        }

        public void UnRegister(Action<T> onEvent) => _onEvent -= onEvent;

        public void Trigger(T t) => _onEvent?.Invoke(t);
    }
    
    public struct EventUnRegister: IUnRegister
    {
        private Action _onUnRegister;

        public EventUnRegister(Action onUnRegister) => _onUnRegister = onUnRegister;
        
        public void UnRegister()
        {
            _onUnRegister?.Invoke();
            _onUnRegister = null;
        }
    }

    public class EventContainer
    {
        private Dictionary<Type, IEvent> _typeEventDic = new Dictionary<Type, IEvent>();

        public void Add<T>() where T : IEvent, new() => _typeEventDic.Add(typeof(T), new T());
        
        public T Get<T>() where T : IEvent => _typeEventDic.TryGetValue(typeof(T), out var e) ? (T)e : default;

        public T GetOrAdd<T>() where T : IEvent, new()
        {
            var eventType = typeof(T);
            if (_typeEventDic.TryGetValue(eventType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            _typeEventDic.Add(eventType, t);
            return t;
        }
    }
    
    #endregion

    #region EventTrigger

    public abstract class UnRegisterTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> _unRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister) => _unRegisters.Add(unRegister);

        public void RemoveUnRegister(IUnRegister unRegister) => _unRegisters.Remove(unRegister);

        public void UnRegister()
        {
            foreach (var unRegister in _unRegisters)
            {
                unRegister.UnRegister();
            }

            _unRegisters.Clear();
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy() => UnRegister();
    }
    
    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDisable() => UnRegister();
    }

    public static class UnRegisterExtension
    {
        public static IUnRegister UnRegisterWhenGameObjectOnDestroy(this IUnRegister unRegister,
            UnityEngine.GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out UnRegisterOnDestroyTrigger trigger))
            {
                trigger.AddUnRegister(unRegister);
            }
            else
            {
                gameObject.AddComponent<UnRegisterOnDestroyTrigger>().AddUnRegister(unRegister);
            }

            return unRegister;
        }

        public static IUnRegister UnRegisterWhenGameObjectOnDestroy<T>(this IUnRegister unRegister, T component)
            where T : UnityEngine.Component => unRegister.UnRegisterWhenGameObjectOnDestroy(component.gameObject);
        
        public static IUnRegister UnRegisterWhenGameObjectOnDisable(this IUnRegister unRegister,
            UnityEngine.GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out UnRegisterOnDisableTrigger trigger))
            {
                trigger.AddUnRegister(unRegister);
            }
            else
            {
                gameObject.AddComponent<UnRegisterOnDisableTrigger>().AddUnRegister(unRegister);
            }

            return unRegister;
        }

        public static IUnRegister UnRegisterWhenGameObjectOnDisable<T>(this IUnRegister unRegister, T component)
            where T : UnityEngine.Component => unRegister.UnRegisterWhenGameObjectOnDisable(component.gameObject);
    }
    
    #endregion
}
