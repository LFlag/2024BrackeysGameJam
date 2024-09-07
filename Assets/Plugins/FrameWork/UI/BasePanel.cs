using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 面板基类 直接代码关联面板的所有控件 并且为子类提供了方便的处理逻辑
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BasePanel : MonoBehaviour
    {
        // 存储面板控件
        private Dictionary<string, List<UIBehaviour>> _controlDic = new Dictionary<string, List<UIBehaviour>>();

        protected CanvasGroup CanvasGroup;

        [SerializeField] [Header("淡入淡出速度")] protected float alphaSpeed = 10f;

        // 淡入结束后执行的回调函数
        protected UnityAction HideCallBack;

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            FindChildrensInControl<Button>();
            FindChildrensInControl<Image>();
            FindChildrensInControl<Toggle>();
            FindChildrensInControl<Text>();
            FindChildrensInControl<Slider>();
            FindChildrensInControl<InputField>();
            FindChildrensInControl<ScrollRect>();
        }

        protected virtual void OnClick(string btnName) { }

        protected virtual void OnValueChanged(string toggleName, bool value) { }
        
        protected virtual void OnValueChanged(string toggleName, float value) { }

        /// <summary>
        /// 得到对应名字的控件组件
        /// </summary>
        /// <param name="controlName"> 控件名字 </param>
        /// <typeparam name="T"> 控件类型 </typeparam>
        /// <returns> 控件 </returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (_controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < _controlDic[controlName].Count; ++i)
                {
                    if (_controlDic[controlName][i] is T)
                        return _controlDic[controlName][i] as T;
                }
            }

            return null;
        }

        /// <summary>
        /// 得到面板子对象的对应控件
        /// </summary>
        /// <typeparam name="T"> 控件类型 </typeparam>
        void FindChildrensInControl<T>() where T : UIBehaviour
        {
            var controls = GetComponentsInChildren<T>();

            for (int i = 0; i < controls.Length; ++i)
            {
                var objName = controls[i].gameObject.name;
                // 如果存在该控件组件的物体 则将该控件添加进物体对应的容器
                if (_controlDic.ContainsKey(objName))
                    _controlDic[objName].Add(controls[i]);
                else
                    // 否则先创建该物体且将控件加入对应的容器
                    _controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });

                switch (controls[i])
                {
                    case Button:
                        (controls[i] as Button)?.onClick.AddListener(() => { OnClick(objName); });
                        break;
                    case Toggle:
                        (controls[i] as Toggle)?.onValueChanged.AddListener((value) => { OnValueChanged(objName, value); });
                        break;
                    case Slider:
                        (controls[i] as Slider)?.onValueChanged.AddListener((value) => { OnValueChanged(objName, value); });
                        break;
                }
            }
        }


        /// <summary>
        /// 淡入隐藏面板
        /// </summary>
        /// <param name="callBack"> 面板隐藏后执行的回调函数 </param>
        public virtual void HideMe(UnityAction callBack = null)
        {
            StartCoroutine(AlphaChange(1f, 0f));
            HideCallBack = callBack;
        }

        // 淡出显示面板
        public virtual void ShowMe() => StartCoroutine(AlphaChange(0f, 1f));

        /// <summary>
        /// 用以面板的淡入淡出
        /// </summary>
        /// <param name="beginAlpha"> 起始透明度 </param>
        /// <param name="endAlpha"> 目的透明度 </param>
        /// <returns></returns>
        protected IEnumerator AlphaChange(float beginAlpha, float endAlpha)
        {
            CanvasGroup.alpha = beginAlpha;
            // 淡出操作
            if (beginAlpha < endAlpha)
            {
                while (CanvasGroup.alpha < endAlpha)
                {
                    // 近似计算 淡出完成
                    if (CanvasGroup.alpha > endAlpha - 0.1f)
                        CanvasGroup.alpha = endAlpha;

                    CanvasGroup.alpha += Time.deltaTime * alphaSpeed;
                    yield return null;
                }
            }
            // 淡入操作
            else
            {
                while (CanvasGroup.alpha > endAlpha)
                {
                    // 近似计算 淡入完成
                    if (CanvasGroup.alpha < endAlpha + 0.1f)
                    {
                        CanvasGroup.alpha = endAlpha;
                        HideCallBack?.Invoke();
                    }

                    CanvasGroup.alpha -= Time.deltaTime * alphaSpeed;
                    yield return null;
                }
            }
        }
    }
}