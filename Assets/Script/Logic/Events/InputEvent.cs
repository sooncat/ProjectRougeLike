using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 用户输入事件，点击/拖拽/滑屏等
/// 由View发出，Logic接收
/// 注意此类事件的参数应尽量使用基本数据，如int/bool等，方便模拟输入
/// </summary>
public enum InputEvent : int
{

    NoneStart = 40000,

    LoginLogin,

    CityStartFight,
    CityExit,

    StageNodeClicked,           //地图界面，点击节点
    StageHeroNodeClicked,       //地图界面，点击英雄节点
    FightNodeDetailComfirmed,   //地图界面，点击战斗详情界面的确定
    RewardNodeDetailComfirmed,  //地图界面，点击奖励详情界面的确定
    HeroNodeDetailComfirmed,    //地图界面，点击英雄详情界面的确定
    RewardNodeGet,              //地图界面，获取奖励

    FightNodeClicked,   //点击战斗地图中的节点
    FightReady,         //战斗地图界面，Ready按钮
    FightExit,          //战斗地图界面，返回按钮
    FightDragOnNode,    //战斗地图界面，拖拽到普通节点上
    FightDragOnHero,    //战斗地图界面，拖拽到英雄节点上
    FightDragAllOnNode, //战斗地图界面，将（安全屋内）所有英雄拖拽到普通节点（一般是Boss）上

    FightAttack,        //战斗中，普通攻击（英雄只能对敌人攻击）
    FightSelectHero,    //战斗中，选择英雄
    FightItemClicked,   //战斗中，点击道具

    FightUseItemToEnemy,     //战斗中，对敌人使用道具
    FightUseItemToHero,      //战斗中，对英雄使用道具

    FightWinConfirm,        //一个节点战斗胜利
    FightLoseConfirm,       //一个节点战斗失败
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