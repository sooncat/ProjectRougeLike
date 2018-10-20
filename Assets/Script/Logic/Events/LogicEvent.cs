using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

/// <summary>
/// 逻辑事件
/// 一般是Logic发出，Logic接收
/// </summary>
public enum LogicEvent : int
{
    None = 0,

    SceneLoadStart,
    SceneLoadProgressChanged,
    SceneLoadEnd,

    UiLoadStart,
    UiLoadProgressChanged,
    UiLoadEnd,

    AllPreLoadEnd,

    UiLoadingStart,
    UiLoadingUpdate,
    UiLoadingEnd,

    ChangeState,
    LeaveState,
    EnterState,

    
    CreateFightStageData,//进入关卡时创建战斗数据
    CreateFightHeroData,//进入关卡时创建英雄数据

    StartFightRound,    //进入回合战斗

    FightStartAi,   //开启AI，决定机器人动作

    FightLoseReturnToStage, //一场战斗失败
    

}

public enum NetEvent:int
{
    CreatePlayerDatas = 1000,
}
