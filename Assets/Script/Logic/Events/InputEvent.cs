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

    FightNodeClicked,   //点击战斗地图中的节点
    FightReady,         //战斗界面，Ready按钮
    FightExit,          //战斗界面，返回按钮
    FightDragOnNode,          //战斗界面，拖拽到普通节点上
    FightDragOnHero,          //战斗界面，拖拽到英雄节点上
    

}
