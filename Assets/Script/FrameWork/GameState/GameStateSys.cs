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
        EventSys.Instance.AddHander(LogicEvent.LeaveState, OnLeaveState);
        
    }

    private List<BaseGameState> _gameStateList;
    private BaseGameState _nowState;
    private Type _willStateType;
    private object _willStateParameter;

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


    void OnChangeState(int id, object p1, object p2)
    {
        Type t = (Type)p1;
        _willStateParameter = null;
        if(p2!=null)
        {
            _willStateParameter = p2;
        }
        
        ChangeState(t);
    }

    void ChangeState(Type type)
    {
        if (_nowState == null)
        {
            _nowState = GetState(type);
            _nowState.Enter(_willStateType);
            _willStateType = null;
            return;
        }

        if (_nowState.GetType() == type)
        {
            return;
        }

        if (_willStateType != null && _willStateType == type)
        {
            return;
        }

        _willStateType = type;
        _nowState.Leave();
    }

    void OnLeaveState(int id, object p1, object p2)
    {
        Type t = (Type)p1;
        Debug.Log("OnLeaveState" + t.Name + ", will = " + _willStateType.Name);
        if(_nowState.GetType() == t)
        {
            _nowState = GetState(_willStateType);
            _nowState.Enter(_willStateParameter);
            _willStateType = null;
        }
    }

    public override void SysUpdate()
    {
        base.SysUpdate();
        if(_nowState!= null)
            _nowState.GameUpdate();
    }
}
