using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 资源管理的主要功能
/// 1.同步加载（一般小资源）
/// 2.异步加载（网络资源或者大资源）
/// 4.对象池（实例的创建与销毁）
/// 5.热更新资源的管理,这个功能可以独立出去
/// </summary>
public class ResourceSys : ISystem
{

    public static ResourceSys Instance = null;
    
    public override void Init()
    {
        base.Init();
        
        _objectPool = new Dictionary<int, List<PoolObject>>();
        _objectDic = new Dictionary<int, PoolObject>();
        Instance = this;
    }

    public void Clear()
    {
        
    }


    public GameObject CreateUI(string uiPrefabPath)
    {
        //todo: load and ins obj.
        return null;
    }
    
    public Sprite GetSprite(string iconPath)
    {
        //if(!iconPath.EndsWith(".png"))
        //{
        //    iconPath += ".png";
        //}
        Sprite result = Resources.Load<Sprite>(iconPath);
        if(result == null)
        {
            throw new FileNotFoundException(iconPath);
        }
        return result;
    }

    public Sprite GetFrame(int lv)
    {
        string framePath = GameConstants.FramePath + lv;
        return GetSprite(framePath);
    }
    

    //-----------------pool

    class PoolObject
    {
        public int KeyId;
        public GameObject CacheObj;
        public bool IsInUse;

    }

    Dictionary<int, List<PoolObject>> _objectPool;
    Dictionary<int, PoolObject> _objectDic;
    
    /// <summary>
    /// 加载并实例化
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    //public GameObject InsObject(string path)
    //{
        
    //}

    /// <summary>
    /// 已有一个model，实例化更多
    /// </summary>
    /// <param name="rootGo"></param>
    /// <returns></returns>
    public GameObject InsPoolObject(GameObject rootGo)
    {
        int insId = rootGo.GetInstanceID();

        if (_objectPool.ContainsKey(insId))
        {
            foreach (PoolObject obj in _objectPool[insId])
            {
                if (!obj.IsInUse)
                {
                    obj.CacheObj.SetActive(true);
                    return obj.CacheObj;
                }
            }
        }
        else
        {
            _objectPool.Add(insId, new List<PoolObject>());
        }
        //
        GameObject result = GameObject.Instantiate(rootGo);
        PoolObject po = new PoolObject();
        po.CacheObj = result;
        po.IsInUse = true;
        po.KeyId = rootGo.GetInstanceID();
        _objectPool[insId].Add(po);
        _objectDic.Add(result.GetInstanceID(), po);

        return result;
    }

    void DestroyPoolObject(GameObject instantiateGo)
    {
        PoolObject po;
        _objectDic.TryGetValue(instantiateGo.GetInstanceID(), out po);
        po.CacheObj.SetActive(false);
        po.IsInUse = false;
    }

    void ClearPoolObject(GameObject rootGo)
    {
        int key = rootGo.GetInstanceID();
        if (_objectPool.ContainsKey(key))
        {
            foreach (PoolObject poolObject in _objectPool[key])
            {
                _objectDic.Remove(poolObject.CacheObj.GetInstanceID());
                Destroy(poolObject.CacheObj);
            }
        }
        _objectPool.Remove(key);
    }

    void ClearPoolAll()
    {
        foreach (KeyValuePair<int, List<PoolObject>> pair in _objectPool)
        {
            foreach (PoolObject poolObject in pair.Value)
            {
                Destroy(poolObject.CacheObj);
            }
        }
        _objectPool.Clear();
        _objectDic.Clear();
    }

}
