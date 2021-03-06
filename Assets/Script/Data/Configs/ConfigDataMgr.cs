﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using com.initialworld.framework;
using UnityEngine;

/// <summary>
/// 配置表管理
/// </summary>
public class ConfigDataMgr {

    //HeroDataConfig _heroDataConfigs;
    //ItemDataConfig _itemDataConfigs;
    //MonsterDataConfig _monsterDataConfigs;

    Dictionary<Type, Dictionary<int, BaseDataInfo>> _dataConfigs;

    string _assetbundlePath;

    static ConfigDataMgr _instance;
    public static ConfigDataMgr Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ConfigDataMgr();
            }
            return _instance;
        }
    }

    private ConfigDataMgr()
    {
        _assetbundlePath = Application.streamingAssetsPath + "AssetBundls/configs/tabledata";
        _dataConfigs = new Dictionary<Type, Dictionary<int, BaseDataInfo>>();
    }

    //public void PreLoad()
    //{
    //    Load<HeroDataConfig>();
    //    Load<ItemDataConfig>();
    //    Load<MonsterDataConfig>();
    //}

    public void Load<T>() where T : IDataConfig
    {
        //todo 改为异步&加密
        Type t = typeof(T);
        string path = GameConstants.AssetBundlePath + "configs/tabledata";
        AssetBundle ab = AssetBundleSys.Instance.LoadAssetBundleInStreaming(path);
        TextAsset ta = ab.LoadAsset<TextAsset>(t.Name + ".json");

        //if (File.Exists("Assets/"+resPath))
        //{
            //string s = Resources.Load<TextAsset>(resPath).text;
            IDataConfig resDataConfig = LitJson.JsonMapper.ToObject<T>(ta.text);
            
            IList dataList = resDataConfig.GetDataInfoList();
            Dictionary<int, BaseDataInfo> tableData = new Dictionary<int, BaseDataInfo>();
            foreach (BaseDataInfo dataDetail in dataList)
            {
                FieldInfo idFieldinfo = typeof(BaseDataInfo).GetField("Id");
                int key = (int)idFieldinfo.GetValue(dataDetail);
                tableData.Add(key, dataDetail);
            }
            _dataConfigs.Add(typeof(T), tableData);
        //}
        //else
        //{
        //    throw new FileNotFoundException("Don't Find Type Res named " + resPath);    
        //}
    }

    public BaseDataInfo GetDataInfo<T>(int id) where T : IDataConfig
    {
        Dictionary<int, BaseDataInfo> dataTable = _dataConfigs[typeof(T)];
        BaseDataInfo result;
        bool hasKey = dataTable.TryGetValue(id, out result);
        if (!hasKey)
        {
            throw new ArgumentException("Don't Find Data With Id = " + id);
        }
        return result;
    }

    public Dictionary<int, BaseDataInfo> GetData<T>() where T : IDataConfig
    {
        Dictionary<int, BaseDataInfo> dataTable = _dataConfigs[typeof(T)];
        return dataTable;
    }
}
