using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

public class ConfigSys : ISystem {

    public static ConfigSys Instance;
    private bool _isInited = false;
    public override void Init()
    {
        if (_isInited)
        {
            return;
        }
        _isInited = true;

        Instance = this;
        _configs = new Dictionary<Type, object>();
    }

    Dictionary<Type, object> _configs;

    public void InitJsonConfig<T>(string filePath)
    {
        byte[] b = IOUtils.ReadFile(filePath);
        string str = System.Text.Encoding.UTF8.GetString(b);
        T result = JsonMapper.ToObject<T>(str);
        _configs.Add(typeof(T), result);
    }

    public T GetConfig<T>()
    {
        Type t = typeof(T);
        if(_configs.ContainsKey(t))
        {
            return (T)_configs[t];
        }
        throw new Exception("Don't Inited config " + t.Name);
    }

}
