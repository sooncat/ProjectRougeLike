using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSys : ISystem {

    public static ViewSys Instance = null;
    private bool _isInited = false;
    public override void Init()
    {
        if (_isInited)
        {
            return;
        }
        _isInited = true;
        Instance = this;
        _allUiClass = new Dictionary<string, BaseView>();

        EventSys.Instance.AddHander(LogicEvent.UiLoadStart, OnUiPreLoad);
    }

    Dictionary<string, BaseView> _allUiClass;

    public void RegistUiClass<T>() where T : BaseView,new()
    {
        _allUiClass.Add(typeof(T).Name, new T());
    }

    void OnUiPreLoad(object p1, object p2)
    {
        var preLoadUis = (List<GameStateConfig.PreLoadResConfig>) p1;
        foreach (GameStateConfig.PreLoadResConfig uiConfig in preLoadUis)
        {
            //CatDebug.LogFunc(uiConfig.Prefab);
            GameObject go = GameObject.Find(uiConfig.Prefab);
            if(!go)
            {
                go = ResourceSys.Instance.CreateUI(uiConfig.Prefab);
            }
            Type t = Type.GetType(uiConfig.ClassName);
            BaseView baseView = (BaseView)go.AddComponent(t);
            baseView.InitUI(go.GetComponent<UINode>());
        }

        EventSys.Instance.AddEvent(LogicEvent.UiLoadEnd);
    }

    public void ClearWindow()
    {
        
    }

}
