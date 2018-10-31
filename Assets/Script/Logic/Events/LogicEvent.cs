using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 逻辑事件
/// 一般是Logic发出，Logic接收
/// </summary>
public enum LogicEvent : int
{
    None = 0,

    SceneLoadStart,             //场景加载
    SceneLoadProgressChanged,
    SceneLoadEnd,

    PreLoadStart,               //相关资源预加载，可能会陆续添加其他资源
    PreLoadProgressChanged,
    PreLoadEnd,

    UiInsStart,                //UI实例化,之后可能会陆续添加其他资源实例化？
    UiInsProgressChanged,
    UiInsEnd,

    AllLoadEnd,                 //资源加载完成

    ChangeState,
    LeaveState,
    EnterState,

    
    CreateFightStageData,//进入关卡时创建战斗数据
    CreateFightHeroData,//进入关卡时创建英雄数据

    StartFightRound,    //进入回合战斗

    FightStartAi,   //开启AI，决定机器人动作

    FightLoseReturnToStage, //一场战斗失败
    FightWinReturnToStage, //一场战斗胜利
    

}

public enum NetEvent:int
{
    CreatePlayerDatas = 1000,
}
