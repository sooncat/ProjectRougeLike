using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class GameStateSys : ISystem {


    public static GameStateSys Instance;
    private bool _isInited;
    public override void Init()
    {
        if (_isInited)
        {
            return;
        }
        _isInited = true;

        _gameStateList = new List<BaseGameState>();

        Instance = this;

        EventSys.Instance.AddHander(LogicEvent.ChangeState, OnChangeState);
        EventSys.Instance.AddHander(LogicEvent.ChangeToNextState, OnChangeToNextState);
        
    }

    private List<BaseGameState> _gameStateList;
    private BaseGameState _nowState;
    private BaseGameState _willState;
    private BaseGameState _leaveState;

    public void InitState<T>() where T:BaseGameState, new()
    {
        BaseGameState gs = new T();
        gs.Init();
        _gameStateList.Add(gs);
    }

    public T GetState<T>()
    {
        foreach (BaseGameState gameState in _gameStateList)
        {
            if (gameState is T)
            {
                return (T)(object)gameState;
            }
        }
        throw new Exception("Don't Find GameState with Type " + typeof(T));
    }

    public BaseGameState GetState(Type type)
    {
        foreach (BaseGameState gameState in _gameStateList)
        {
            if(gameState.GetType() == type)
            {
                return gameState;
            }
        }
        return null;
    }

    void OnChangeToNextState(int id, object p1, object p2)
    {
        ChangeState(_willState.GetType(), true);
    }

    void OnChangeState(int id, object p1, object p2)
    {
        Type t = (Type)p1;
        if(p2!=null)
        {
            bool isLoadingDone = (bool)p2;
            ChangeState(t, isLoadingDone);
        }
        else
        {
            ChangeState(t);
        }
        
    }

    void ChangeState(Type type, bool loadingDone = false)
    {
        if (_nowState != null && _nowState.GetType() == type)
        {
            return;
        }

        if (_willState != null && _willState.GetType() == type && !loadingDone)
        {
            return;
        }

        ChangeStateSecond(type);
    }

    void ChangeStateSecond(Type type)
    {
        //CatDebug.Log("GameRoot::ChangeStateForce() 1 Origin = " + state + ", m_curGameState = " + m_curGameState, CatDebug.LogType.PVE_SCENE);
        if (_nowState == null)
        {
            _nowState = GetState(type);
        }
        else
        {
            if (_nowState.GetType() != type)
            {
                _leaveState = _nowState;
                _leaveState.Leave();
            }
            if (_nowState is LoadingState) //out loading
            {
                _nowState = _willState;
            }
            else            // in loading
            {
                _willState = GetState(type);
                LoadingState loadingState = GetState<LoadingState>();
                loadingState.SetWillState(_willState.GetName());
                _nowState = loadingState;
            }
        }
        //CatDebug.Log("GameRoot::ChangeStateForce() 5 Dest = " + m_curGameState, CatDebug.LogType.PVE_SCENE);
        _nowState.Enter();
    }

    public override void SysUpdate()
    {
        base.SysUpdate();
        if(_nowState!= null)
            _nowState.GameUpdate();
    }
}
