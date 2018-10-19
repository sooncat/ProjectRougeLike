using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 此类事件用于UI更新
/// 一般由Logic发出，View接收
/// </summary>
public enum ViewEvent : int {

    NoneStart = 30000,

    CreateStageView,        //绘制战斗关卡
    
    CreateHeroStartNode,    //战斗前选择英雄，选择了一个英雄出击，在地图上创建此英雄
    ReplaceHeroStartNode,    //战斗前选择英雄，选择了另一个英雄将上一个英雄覆盖
    ResetHeroStartNode,     //战斗前选择英雄，修改一个英雄的出战位置
    ExchangeHeroStartNode,  //战斗前选择英雄，交换两个英雄的出战位置
    RemoveHeroStartNode,    //战斗前选择英雄，将选中英雄下架

    FightSubStateMapping,   //战斗准备完毕，进入地图阶段

    CreateFightView,        //进入战斗，创建战斗界面
    SetSelectedHero,        //切换选中的英雄

}
