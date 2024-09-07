using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace Framework
{
    /// <summary>
    /// Mono管理器 利用Mono控制器 为外部提供使用Update、协程等Mono相关功能
    /// </summary>
    public class MonoMgr : BaseManager<MonoMgr>
    {
        /// <summary>
        /// Mono控制器 提供外部可使用的Update、协程Mono相关功能
        /// </summary>
        private class MonoController : MonoBehaviour
        {
            // Update时需调用的事件
            private UnityAction _updateAction;

            // FixedUpdate时需调用的事件
            private UnityAction _fixedUpdateAction;

            private void Start() => DontDestroyOnLoad(this);

            void Update()
            {
                _updateAction?.Invoke();
            }

            void FixedUpdate() => _fixedUpdateAction?.Invoke();

            // 为Update添加事件
            public void AddUpdateListener(UnityAction action) => _updateAction += action;

            // 为Update移除事件
            public void RemoveUpdateListener(UnityAction action) => _updateAction -= action;

            // 为FixedUpdate添加事件
            public void AddFixedUpdateListener(UnityAction action) => _fixedUpdateAction += action;

            // 为FixedUpdate移除事件
            public void RemoveFixedUpdateListener(UnityAction action) => _fixedUpdateAction -= action;
        }
        
        // 管理的Mono控制器
        private readonly MonoController _controller;

        // 保证Mono控制器的唯一性
        public MonoMgr()
        {
            var obj = new GameObject("MonoController");
            _controller = obj.AddComponent<MonoController>();
        }

        #region 封装一层添加FixedUpdate添加事件、移除事件的功能

        public void AddFixedUpdateListener(UnityAction action) => _controller.AddFixedUpdateListener(action);

        public void RemoveFixedUpdateListener(UnityAction action) => _controller.RemoveFixedUpdateListener(action);

        #endregion

        #region 封装一层添加Update添加事件、移除事件的功能

        public void AddUpdateListener(UnityAction action) => _controller.AddUpdateListener(action);

        public void RemoveUpdateListener(UnityAction action) => _controller.RemoveUpdateListener(action);

        #endregion

        #region 封装一层使用协程相关的功能

        public Coroutine StartCoroutine(string methodName) => _controller.StartCoroutine(methodName);

        public Coroutine StartCoroutine(IEnumerator routine) => _controller.StartCoroutine(routine);

        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value) =>
            _controller.StartCoroutine(methodName, value);

        public void StopCoroutine(IEnumerator routine) => _controller.StopCoroutine(routine);

        public void StopCoroutine(Coroutine routine) => _controller.StopCoroutine(routine);

        public void StopCoroutine(string methodName) => _controller.StopCoroutine(methodName);

        public void StopAllCoroutines() => _controller.StopAllCoroutines();

        #endregion
    }
}
