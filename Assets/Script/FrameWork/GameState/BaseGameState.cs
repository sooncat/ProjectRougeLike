using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameState之间差异很大，需要切换场景Scene
/// </summary>
public class BaseGameState {

    protected GameStateConfig.GameStateDetail GsDetail;

    /// <summary>
    /// 当关卡加载完成时，Loading条显示的进度
    /// </summary>
    protected float SceneLoadPercent = 0.5f;
    protected float UiLoadPercent = 1f;

    public BaseGameState()
    {
        
    }

    public virtual void Init()
    {
        CatDebug.LogFunc();
        GameStateConfig GsConfig = ConfigSys.Instance.GetConfig<GameStateConfig>();


        GsConfig.GameStateDetails.TryGetValue(GetName(), out GsDetail);

        if(GsDetail == null)
        {
            throw new KeyNotFoundException(GetName());
        }

        
    }
    public void BaseReset() { Reset(); }
    public virtual void Reset() { }
    public virtual void Enter(GameStateParameter parameter)
    {
        CatDebug.LogFuncInStack(1);

        EventSys.Instance.AddHander(LogicEvent.SceneLoadEnd, OnSceneLoaded);
        EventSys.Instance.AddHander(FrameEvent.PreloadEnd, OnPreLoadEnd);
        EventSys.Instance.AddHander(LogicEvent.UiInsEnd, OnUiLoaded);

        EventSys.Instance.AddEvent(LogicEvent.EnterState, GetType());

        StartLoad();
    }

    public virtual void Leave()
    {
        CatDebug.LogFuncInStack(1);
        EventSys.Instance.RemoveHander(this);
        EventSys.Instance.AddEvent(LogicEvent.UiLoadingStart);
        StartUnLoad();
        
    }

    public virtual void GameUpdate() { }
    
    public string GetName()
    {
        return GetType().Name;
    }

    protected void StartUnLoad()
    {
        //Resources.UnloadUnusedAssets();
        //AudioSys.Instance.UnloadSounds();
        //LevelAgainLeave();
        //Time.timeScale = 1;
        EventSys.Instance.AddEvent(LogicEvent.LeaveState, GetType());
        EventSys.Instance.AddEvent(FrameEvent.ClearAssetBundleChche, true);
    }

    protected void StartLoad()
    {
        StartLoadScene();
    }

    protected void StartLoadScene()
    {
        CatDebug.LogFunc();
        string sceneName = GsDetail.SceneName;
        if (!string.IsNullOrEmpty(sceneName))
        {
            EventSys.Instance.AddEvent(LogicEvent.SceneLoadStart, sceneName, false);
        }
        else
        {
            EventSys.Instance.AddEvent(LogicEvent.SceneLoadEnd, sceneName);
        }
    }

    protected void StartInsUi()
    {
        CatDebug.LogFunc();
        if (GsDetail.PreLoadUi.Count > 0)
        {
            EventSys.Instance.AddEvent(LogicEvent.UiInsStart, GsDetail.PreLoadUi);
        }
        else
        {
            EventSys.Instance.AddEvent(LogicEvent.UiInsEnd);
        }
    }

    protected virtual void OnSceneLoaded(object p1, object p2)
    {
        //EventSys.Instance.AddEvent(LogicEvent.UiLoadingUpdate, SceneLoadPercent);
        //StartLoadUi();
        StartPreLoad();
    }

    void StartPreLoad()
    {
        CatDebug.LogFunc();
        foreach (GameStateConfig.PreLoadResConfig preLoadResConfig in GsDetail.PreLoadUi)
        {
            string path = Application.streamingAssetsPath + "/AssetBundles/" + preLoadResConfig.AssetBundle;
            EventSys.Instance.AddEvent(FrameEvent.AddPreLoadRes, path);
        }
        EventSys.Instance.AddEvent(FrameEvent.PreLoadStart);
    }

    protected virtual void OnPreLoadEnd(object p1, object p2)
    {
        StartInsUi();
    }

    protected virtual void OnUiLoaded(object p1, object p2)
    {
        CatDebug.LogFuncInStack(1);
        //EventSys.Instance.AddEvent(LogicEvent.UiLoadingUpdate, UiLoadPercent);
        OnAllPreLoaded();
    }

    protected virtual void OnAllPreLoaded()
    {
        CatDebug.LogFuncInStack(1);
        EventSys.Instance.AddEvent(LogicEvent.UiLoadingEnd);
    }
}
