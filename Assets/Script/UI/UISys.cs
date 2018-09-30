using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISys : ISystem {

    public static UISys Instance = null;
    private bool _isInited = false;
    public override void Init()
    {
        if (_isInited)
        {
            return;
        }
        _isInited = true;
        Instance = this;
        _allUiClass = new Dictionary<string, BaseUI>();

        EventSys.Instance.AddHander(LogicEvent.UiPreLoadStart, OnUiPreLoad);
    }

    Dictionary<string, BaseUI> _allUiClass;

    public void RegistUiClass<T>() where T : BaseUI,new()
    {
        _allUiClass.Add(typeof(T).Name, new T());
    }

    void OnUiPreLoad(int id, object p1, object p2)
    {
        string stateName = (string)p1;
        GameStateConfig gsConfig = ConfigSys.Instance.GetConfig<GameStateConfig>();
        var preLoadUis = gsConfig.GameStateDetails[stateName].PreLoadUi;
        foreach (GameStateConfig.PreLoadResConfig uiConfig in preLoadUis)
        {
            GameObject go = GameObject.Find(uiConfig.Prefab);
            if(!go)
            {
                go = ResourceSys.Instance.CreateUI(uiConfig.Prefab);
            }
            Type t = Type.GetType(uiConfig.ClassName);
            BaseUI baseUi = (BaseUI)go.AddComponent(t);
            baseUi.InitUI(go.GetComponent<UINode>());
        }

        EventSys.Instance.AddEvent(LogicEvent.UiPreLoadEnd);
    }

    public void ClearWindow()
    {
        
    }

}
