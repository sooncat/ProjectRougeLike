using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

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

    CreateStageView,//绘制战斗关卡
    
    CreateFightStageData,//进入关卡时创建战斗数据
    CreateFightHeroData,//进入关卡时创建英雄数据
}

public enum NetEvent:int
{
    CreatePlayerDatas = 10000,
}
