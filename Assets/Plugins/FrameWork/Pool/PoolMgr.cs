using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 缓存池中的单个类别的容器
public class PoolContianer
{
    // 窗口显示的容器节点
    private GameObject contianerNode;
    // 容器本身
    private Queue<GameObject> contianer;

    // 创建该类容器 同时在缓存池节点下创建容器节点 并将物体加入到容器中
    public PoolContianer(GameObject item, Transform poolNode)
    {
        contianerNode = new GameObject(item.name + "Node");
        contianerNode.transform.parent = poolNode;
        contianer = new Queue<GameObject>();
        PushItem(item);
    }

    // 判断该类容器是否为空
    public bool IsEmpty() => contianer.Count == 0;

    // 从容器中取出该类物体
    public GameObject PopItem(string name)
    {
        GameObject item = contianer.Dequeue();
        item.SetActive(true);
        item.transform.parent = null;
        return item;
    }

    // 将该类物体加入容器中
    public void PushItem(GameObject item)
    {
        item.SetActive(false);
        item.transform.parent = contianerNode.transform;
        contianer.Enqueue(item);
    }
}

// 缓存池管理器
public class PoolMgr : BaseManager<PoolMgr>
{
    // 窗口上的缓存池
    private GameObject poolNode;
    // 缓存池本身
    private Dictionary<string, PoolContianer> poolDic = new Dictionary<string, PoolContianer>();

    // 从池中取出该类物体
    public void PopItem(string name, UnityAction<GameObject> callBack = null)
    {
        if (poolDic.ContainsKey(name) && !poolDic[name].IsEmpty())
            callBack?.Invoke(poolDic[name].PopItem(name));
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
        if (poolNode == null)
            poolNode = new GameObject("Pool");

        string name = item.name;
        // 如果有该类容器 直接添加进容器
        if (poolDic.ContainsKey(name))
            poolDic[name].PushItem(item);
        // 否则创建该类容器
        else
            poolDic.Add(name, new PoolContianer(item, poolNode.transform));
    }

    // 清空缓存池
    public void Clear()
    {
        poolDic.Clear();
        poolNode = null;
    }
}
