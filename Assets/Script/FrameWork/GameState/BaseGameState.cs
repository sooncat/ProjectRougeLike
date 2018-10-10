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
    public virtual void Enter(object parameter)
    {
        CatDebug.LogFunc(1);

        EventSys.Instance.AddHander(LogicEvent.SceneLoadEnd, OnSceneLoaded);
        EventSys.Instance.AddHander(LogicEvent.UiLoadEnd, OnUiLoaded);

        EventSys.Instance.AddEvent(LogicEvent.EnterState, GetType());

        StartLoad();
    }

    public virtual void Leave()
    {
        CatDebug.LogFunc(1);
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

    protected void StartLoadUi()
    {
        CatDebug.LogFunc();
        if (GsDetail.PreLoadUi.Count > 0)
        {
            EventSys.Instance.AddEvent(LogicEvent.UiLoadStart, GsDetail.PreLoadUi);
        }
        else
        {
            EventSys.Instance.AddEvent(LogicEvent.UiLoadEnd);
        }
    }

    protected virtual void OnSceneLoaded(int id, object p1, object p2)
    {
        //EventSys.Instance.AddEvent(LogicEvent.UiLoadingUpdate, SceneLoadPercent);
        StartLoadUi();
    }

    protected virtual void OnUiLoaded(int id, object p1, object p2)
    {
        CatDebug.LogFunc(1);
        //EventSys.Instance.AddEvent(LogicEvent.UiLoadingUpdate, UiLoadPercent);
        OnAllPreLoaded();
    }

    protected virtual void OnAllPreLoaded()
    {
        CatDebug.LogFunc(1);
        EventSys.Instance.AddEvent(LogicEvent.UiLoadingEnd);
    }
}
