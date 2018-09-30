using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingState : BaseGameState {

    string _nextStateName;

    /// <summary>
    /// from 0 to 100
    /// </summary>
    private int _loadingCount;
    private int LoadingCount
    {
        get{return _loadingCount;}
        set
        {
            if(_loadingCount != value)
            {
                _loadingCount = value;
                EventSys.Instance.AddEvent(UIEvent.LoadingPercentChanged, _loadingCount/100.0f);
            }
        }
    }

    public void SetWillState(string nextStateName)
    {
        _nextStateName = nextStateName;
    }

    public override void Init()
    {
        base.Init();
        EventSys.Instance.AddHander(LogicEvent.SceneLoadProgressChanged, OnSceneLoading);
        EventSys.Instance.AddHander(LogicEvent.EndSceneLoad, OnSceneLoaded);
        EventSys.Instance.AddHander(LogicEvent.UiPreLoadEnd, OnUiLoaded);
    }

    void OnSceneLoading(int id, object p1, object p2)
    {
        float p = (float)p1;
        LoadingCount = Mathf.CeilToInt(p * 50);
    }

    void OnSceneLoaded(int id, object p1, object p2)
    {
        LoadingCount = 50;
        EventSys.Instance.AddEvent(LogicEvent.UiPreLoadStart, _nextStateName);
    }

    void OnUiLoaded(int id, object p1, object p2)
    {
        LoadingCount = 100;
        EventSys.Instance.AddEvent(LogicEvent.AllPreLoadEnd);
        EventSys.Instance.AddEvent(LogicEvent.ChangeToNextState);
    }

    public override void Enter()
    {
        base.Enter();
        EventSys.Instance.AddEvent(LogicEvent.EnterLoadingState);
        GameStateConfig gsConfig = ConfigSys.Instance.GetConfig<GameStateConfig>();
        string sceneName = gsConfig.GameStateDetails[_nextStateName].SceneName;
        EventSys.Instance.AddEvent(LogicEvent.StartSceneLoad, sceneName, false);
    }

    public override void Leave()
    {
        base.Leave();
        EventSys.Instance.AddEvent(LogicEvent.LeaveLoadingState);
    }
}
