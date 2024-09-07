using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 缓存池中的单个类别的容器类
    /// </summary>
    public class PoolContainer
    {
        // 窗口显示的容器节点
        private readonly GameObject _containerNode;

        // 容器本身
        private readonly Queue<GameObject> _container;

        // 创建该类容器 同时在缓存池节点下创建容器节点 并将物体加入到容器中
        public PoolContainer(GameObject item, Transform poolNode)
        {
            _containerNode = new GameObject(item.name + "Node");
            _containerNode.transform.parent = poolNode;
            _container = new Queue<GameObject>();
            PushItem(item);
        }

        // 判断该类容器是否为空
        public bool IsEmpty() => _container.Count == 0;

        // 从容器中取出该类物体
        public GameObject PopItem(string name)
        {
            var item = _container.Dequeue();
            item.SetActive(true);
            item.transform.parent = null;
            return item;
        }

        // 将该类物体加入容器中
        public void PushItem(GameObject item)
        {
            item.SetActive(false);
            item.transform.parent = _containerNode.transform;
            _container.Enqueue(item);
        }
    }

    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolMgr : BaseManager<PoolMgr>
    {
        // 窗口上的缓存池
        private GameObject _poolNode;

        // 缓存池本身
        private readonly Dictionary<string, PoolContainer> _poolDic = new Dictionary<string, PoolContainer>();

        // 从池中取出该类物体
        public void PopItem(string name, UnityAction<GameObject> callBack = null)
        {
            if (_poolDic.ContainsKey(name) && !_poolDic[name].IsEmpty())
                callBack?.Invoke(_poolDic[name].PopItem(name));
            else
                ResMgr.Instance.LoadAsync<GameObject>("Pool/" + name, (obj) =>
                {
                    obj.name = name;
                    callBack?.Invoke(obj);
                });
        }

        // 将该类物体加入池中
        public void PushItem(GameObject item)
        {
            // 创建缓存池节点在窗口上
            if (_poolNode == null)
                _poolNode = new GameObject("Pool");

            var name = item.name;
            // 如果有该类容器 直接添加进容器
            if (_poolDic.TryGetValue(name, out var container))
                container.PushItem(item);
            else // 否则创建该类容器
                _poolDic.Add(name, new PoolContainer(item, _poolNode.transform));
        }

        // 清空缓存池
        public void Clear()
        {
            _poolDic.Clear();
            _poolNode = null;
        }
    }
}
