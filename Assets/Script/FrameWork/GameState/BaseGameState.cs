using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameState之间差异很大，需要切换场景Scene
/// </summary>
public class BaseGameState {

    public BaseGameState()
    {
        
    }

    public virtual void Init()
    {
        
    }
    public void BaseReset() { Reset(); }
    public virtual void Reset() { }
    public virtual void Enter()
    {
        CatDebug.LogFunc(1);
    }

    public virtual void Leave()
    {
        LevelAgainLeave();
        Resources.UnloadUnusedAssets();
        AudioSys.Instance.UnloadSounds();
        Time.timeScale = 1;
        CatDebug.LogFunc(1);
    }

    protected virtual void LevelAgainLeave()
    {
        UISys.Instance.ClearWindow();
        ResourceSys.Instance.Clear();

    }

    public virtual void GameUpdate() { }
    
    public string GetName()
    {
        return GetType().Name;
    }

}
