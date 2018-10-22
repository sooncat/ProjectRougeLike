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

    ShowNodeFightDetails,   //地图界面，显示战斗节点详情
    ShowNodeRewardDetails,  //地图界面，显示奖励节点详情
    ShowNodeRewardGet,      //地图界面，英雄被拖拽到奖励节点,显示奖励并准备赋予英雄

    CreateFightView,        //进入战斗，创建战斗界面
    SetSelectedHero,        //切换选中的英雄
    FightUpdateRound,       //进入战斗，下一回合
    FightHeroAttack,        //战斗中玩家攻击
    FightUpdateEnemyState,  //战斗中更新敌人状态
    FightShowWin,           //战斗胜利界面
    FightShowLose,          //战斗失败界面
    FightWinReturnToStage,  //战斗胜利，返回地图界面
    FightLoseReturnToStage, //战斗失败，返回地图界面
    FightChangeTurn,        //一个回合中，切换主动权
    FightEnemyAttack,       //战斗中敌人攻击
    FightUpdateHeroState,   //战斗中更新英雄状态

    ShowStageFail,          //关卡失败

    ShowTipNodePassed,          //地图界面，提示此点已经被攻略
    ShowTipNotNextNode,         //地图界面，提示此点不是当前节点的子节点
    GetRewardReturnToStage,     //地图界面，获取奖励后刷新
}
