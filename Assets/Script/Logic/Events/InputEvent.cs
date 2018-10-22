using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 用户输入事件，点击/拖拽/滑屏等
/// 由View发出，Logic接收
/// </summary>
public enum InputEvent : int
{

    NoneStart = 40000,

    LoginLogin,

    CityStartFight,
    CityExit,

    StageNodeClicked,           //地图界面，点击节点
    FightNodeDetailComfirmed,   //地图界面，点击战斗详情界面的节点
    RewardNodeDetailComfirmed,   //地图界面，点击奖励详情界面的节点
    RewardNodeGet,              //地图界面，获取奖励

    FightNodeClicked,   //点击战斗地图中的节点
    FightReady,         //战斗地图界面，Ready按钮
    FightExit,          //战斗地图界面，返回按钮
    FightDragOnNode,    //战斗地图界面，拖拽到普通节点上
    FightDragOnHero,    //战斗地图界面，拖拽到英雄节点上

    FightAttack,        //战斗中，普通攻击（英雄只能对敌人攻击）
    FightItem,          //战斗中，使用道具（可以对任何人使用道具）
    FightSelectHero,    //战斗中，选择英雄
}

/// <summary>
/// AI的输入
/// </summary>
public enum AiInputEvent:int
{
    NoneStart = 50000,
    Attack,
    UseItem,
    UseSkill,

    AiActionEnd,    //ai行动完毕
}