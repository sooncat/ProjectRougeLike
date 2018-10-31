using System;
using System.Collections;
using System.Collections.Generic;
using com.initialworld.framework;
using Newtonsoft.Json;
using UnityEngine;


public class FightState : BaseGameState {

    string _stageName;
    StageConfig _stageConfig;

    enum FSubState
    {
        SelectHero, //选择英雄
        Mapping,    //地图中
        Fighting,   //战斗中
        Shopping,   //商店
        Result,     //战斗结果     
    }
    FSubState _fState;

    /// <summary>
    /// 被选中准备出战的英雄
    /// { Key：HeroId, Value：NodeId }
    /// </summary>
    Dictionary<int, int> _selectedHeros;

    FightProgress _fightProgress;
    
    public override void Enter(GameStateParameter parameter)
    {
        base.Enter(parameter);

        _fState = FSubState.SelectHero;
        _selectedHeros = new Dictionary<int, int>();

        FightStateParameter fsParameter = (FightStateParameter)parameter;
        _stageName = fsParameter.nextType;

        EventSys.Instance.AddHander(InputEvent.FightExit, OnExitEvent);
        EventSys.Instance.AddHander(InputEvent.FightReady, OnFightReadyEvent);
        EventSys.Instance.AddHander(InputEvent.FightNodeClicked, OnClickFightNodeEvent);
        EventSys.Instance.AddHander(InputEvent.FightDragOnNode, OnFightDragOnNode);
        EventSys.Instance.AddHander(InputEvent.FightDragOnHero, OnFightDragOnHero);
        EventSys.Instance.AddHander(InputEvent.FightDragAllOnNode, OnFightDragAllOnNode);

        EventSys.Instance.AddHander(LogicEvent.FightLoseReturnToStage, OnFightLoseReturnToStage);
        EventSys.Instance.AddHander(LogicEvent.FightWinReturnToStage, OnFightWinReturnToStage);

        EventSys.Instance.AddHander(InputEvent.StageNodeClicked, OnStateNodeClicked);
        EventSys.Instance.AddHander(InputEvent.StageHeroNodeClicked, OnStateHeroNodeClicked);
        
        EventSys.Instance.AddHander(InputEvent.FightNodeDetailComfirmed, (param1, param2) => { });
        EventSys.Instance.AddHander(InputEvent.RewardNodeDetailComfirmed, (param1, param2) => { });
        EventSys.Instance.AddHander(InputEvent.RewardNodeGet, GetReward);
        

    }

    void OnExitEvent(object p1, object p2)
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(CityState));
    }

    void OnFightReadyEvent(object p1, object p2)
    {
        if (_selectedHeros.Count > 0)
        {
            _fState = FSubState.Mapping;
            EventSys.Instance.AddEventNow(LogicEvent.CreateFightHeroData, _selectedHeros);
            EventSys.Instance.AddEvent(ViewEvent.FightSubStateMapping, FightDataMgr.Instance.GetHeros());
        }
        else
        {
            EventSys.Instance.AddEvent(ViewEvent.ShowTipSelectHero);
        }
    }

    void OnClickFightNodeEvent(object p1, object p2)
    {
        
    }

    protected override void OnUiLoaded(object p1, object p2)
    {
        base.OnUiLoaded(p1, p2);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        string path = GameConstants.AssetBundlePath + "configs/stage";
        AssetBundle stageAssetBundle = AssetBundleSys.Instance.LoadAssetBundleInStreaming(path);
        string str = stageAssetBundle.LoadAsset<TextAsset>(_stageName + ".json").text;
        //string str = Resources.Load<TextAsset>(GameConstants.StageConfigPath + _stageName).text;
        _stageConfig = JsonConvert.DeserializeObject<StageConfig>(str, settings);

        EventSys.Instance.AddEvent(LogicEvent.CreateFightStageData, _stageConfig);
        //EventSys.Instance.AddEvent(LogicEvent.CreateFightHeroData);
        EventSys.Instance.AddEvent(ViewEvent.CreateStageView, _stageConfig);

        _fightProgress = new FightProgress(_stageConfig);

        OnAllPreLoaded();
    }

    /// <summary>
    /// 将英雄拖拽到地图节点上
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnFightDragOnNode(object p1, object p2)
    {
        int heroId = (int)p1;
        int targetNodeId = (int)p2;
        
        CatDebug.LogFunc(" p1 = " + heroId + ", p2 = " + targetNodeId);

        switch (_fState)
        {
            case FSubState.SelectHero:
                //拖拽一个英雄到一个无人的起始节点上
                Hero h = PlayerDataMgr.Instance.GetHero(heroId);
                if(h != null)
                {
                    if(_selectedHeros.ContainsKey(h.Id))    //从另一个起始节点拖拽到新起始节点，即替换英雄起始位置
                    {
                        _selectedHeros[h.Id] = targetNodeId;
                        EventSys.Instance.AddEvent(ViewEvent.ResetHeroStartNode, heroId, targetNodeId);
                    }
                    else       //从英雄列表中拖拽到起始节点
                    {
                        _selectedHeros.Add(h.Id, targetNodeId);
                        EventSys.Instance.AddEvent(ViewEvent.CreateHeroStartNode, h.CreatureData, targetNodeId);       
                    }
                }
                break;
            case FSubState.Mapping:

                BaseStageNode node = _stageConfig.GetNode(targetNodeId);

                if(node.IsPassed)
                {
                    EventSys.Instance.AddEvent(ViewEvent.ShowTipNodePassed);
                    return;
                }

                FightHero fh = FightDataMgr.Instance.GetHero(heroId);
                BaseStageNode heroNode = _stageConfig.GetNode(fh.NowNodeId);
                if (!heroNode.NextNodes.Contains(targetNodeId))
                {
                    EventSys.Instance.AddEvent(ViewEvent.ShowTipNotNextNode);
                    return;
                }

                string nodeType = node.NodeType;

                if(nodeType.Equals(typeof(StageNodeFight).Name))
                {
                    EventSys.Instance.AddEvent(LogicEvent.StartFightRound, new []{heroId}, targetNodeId);
                }
                else if(nodeType.Equals(typeof(StageNodeReward).Name))
                {
                    Reward r = FightDataMgr.Instance.GetReward(targetNodeId);
                    EventSys.Instance.AddEvent(ViewEvent.ShowNodeRewardGet, r, new []{targetNodeId, heroId});
                }
                else if(nodeType.Equals(typeof(StageNodeSafe).Name))
                {
                    fh.NowNodeId = targetNodeId;
                    EventSys.Instance.AddEvent(ViewEvent.GetSafeReturnToStage, fh);
                }
                break;
        }
    }

    void OnFightDragAllOnNode(object p1, object p2)
    {
        int fromNodeId = (int)p1;
        int targetNodeId = (int)p2;

        StageNodeSafe fromNode = (StageNodeSafe)_stageConfig.GetNode(fromNodeId);
        BaseStageNode targetNode = _stageConfig.GetNode(targetNodeId);

        if (!fromNode.NextNodes.Contains(targetNodeId))
        {
            EventSys.Instance.AddEvent(ViewEvent.ShowTipNotNextNode);
            return;
        }

        if(targetNode.GetType() !=  typeof(StageNodeFight))
        {
            EventSys.Instance.AddEvent(ViewEvent.ShowTipNotSupportYet);
            return;
        }

        switch (_fState)
        {
            case FSubState.SelectHero:
                break;
            case FSubState.Mapping:
                
                List<int> heros = new List<int>();
                foreach (var pair in FightDataMgr.Instance.GetHeros())
                {
                    if (pair.Value.NowNodeId == fromNode.Id)
                    {
                        heros.Add(pair.Value.Id);
                    }
                }
                EventSys.Instance.AddEvent(LogicEvent.StartFightRound, heros.ToArray(), targetNodeId);
                break;
        }
    }

    /// <summary>
    /// 将英雄拖拽到其他英雄节点上
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnFightDragOnHero(object p1, object p2)
    {
        int dragHeroId = (int)p1;       //
        int targetHeroId = (int)p2; //
        switch (_fState)
        {
            case FSubState.SelectHero:
                if (_selectedHeros.ContainsKey(dragHeroId))
                {
                    //交换两个英雄的起始节点
                    int nodeId = _selectedHeros[dragHeroId];
                    _selectedHeros[dragHeroId] = _selectedHeros[targetHeroId];
                    _selectedHeros[targetHeroId] = nodeId;

                    int[] param1 = new[] { dragHeroId, _selectedHeros[dragHeroId] };
                    int[] param2 = new[] { targetHeroId, _selectedHeros[targetHeroId] };
                    EventSys.Instance.AddEvent(ViewEvent.ExchangeHeroStartNode, param1, param2);
                }
                else
                {   
                    //拖拽一个英雄到有已有英雄存在的起始节点上（替换出战英雄）
                    //p1 = 新选中的出战英雄
                    //p2 = 已经在节点上，需要被替换的英雄
                    Hero newHero = PlayerDataMgr.Instance.GetHero(dragHeroId);
                    //Hero targetHero = PlayerDataMgr.Instance.GetHero(targetHeroId);
                    int nodeId = _selectedHeros[targetHeroId];
                    _selectedHeros.Remove(targetHeroId);
                    _selectedHeros[dragHeroId] = nodeId;
                    EventSys.Instance.AddEvent(ViewEvent.ReplaceHeroStartNode, newHero.CreatureData, new [] { targetHeroId, nodeId });
                }
                break;
            case FSubState.Mapping:
                //将英雄拖拽到英雄身上，除非是安全屋，否则不可能
                FightHero fh = FightDataMgr.Instance.GetHero(targetHeroId);
                BaseStageNode stageNode = _stageConfig.GetNode(fh.NowNodeId);
                if(stageNode.NodeType.Equals(typeof(StageNodeSafe).Name))
                {
                    FightHero dragHero = FightDataMgr.Instance.GetHero(dragHeroId);
                    dragHero.NowNodeId = fh.NowNodeId;
                    EventSys.Instance.AddEvent(ViewEvent.GetSafeReturnToStage, dragHero);
                }
                else
                {
                    EventSys.Instance.AddEvent(ViewEvent.ShowTipNodePassed);
                }
                break;
        }
    }

    /// <summary>
    /// 英雄战斗失败，回到地图界面
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnFightLoseReturnToStage(object p1, object p2)
    {
        //判定是否所有英雄死亡
        bool isAllDied = true;
        foreach (var pair in FightDataMgr.Instance.GetHeros())
        {
            if (pair.Value.CreatureData.Hp.Value > 0)
            {
                isAllDied = false;
            }
        }
        if (isAllDied)
        {
            EventSys.Instance.AddEvent(ViewEvent.ShowStageFail);
        }
    }

    void OnFightWinReturnToStage(object p1, object p2)
    {
        int nodeId = (int)p1;
        bool isLast = _stageConfig.GetNode(nodeId).IsFinalNode;
        if(isLast)
        {
            List<Item> rewards = new List<Item>();
            foreach (int itemId in _stageConfig.ItemIds)
            {
                rewards.Add(new Item(itemId, 1));
            }
            EventSys.Instance.AddEvent(ViewEvent.ShowStageWin, rewards);
        }
    }

    public override void Leave()
    {
        base.Leave();
        _fightProgress.Clear();
    }

    /// <summary>
    /// 单击节点显示详情
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnStateNodeClicked(object p1, object p2)
    {
        int nodeId = (int)p1;
        BaseStageNode node = _stageConfig.GetNode(nodeId);
        if(node.IsPassed)
        {
            return;
        }
        if(node.HiddenDetail)
        {
            EventSys.Instance.AddEvent(ViewEvent.ShowNodeUnKnow, nodeId);
            return;
        }
        Type type = node.GetType();

        if (type == typeof(StageNodeFight))
        {
            Enemy e = FightDataMgr.Instance.GetEnemy(nodeId);
            EventSys.Instance.AddEvent(ViewEvent.ShowNodeFightDetails, e.CreatureData, nodeId);
        }
        else if (type == typeof(StageNodeReward))
        {
            Reward r = FightDataMgr.Instance.GetReward(nodeId);
            EventSys.Instance.AddEvent(ViewEvent.ShowNodeRewardDetails, r, nodeId);
        }
        else if(type == typeof(StageNodeHidden))
        {
            
        }
        else if(type == typeof(StageNodeSafe))
        {
            EventSys.Instance.AddEvent(ViewEvent.ShowNodeSafeDetails, nodeId);
        }
    }

    void OnStateHeroNodeClicked(object p1, object p2)
    {
        int heroId = (int)p1;

        Hero hero = PlayerDataMgr.Instance.GetHero(heroId);
        EventSys.Instance.AddEvent(ViewEvent.ShowHeroNodeDetails, hero.CreatureData, heroId);
    }

    /// <summary>
    /// 从奖励节点拿到奖励
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void GetReward(object p1, object p2)
    {
        int nodeId = (int)p1;
        int heroId = (int)p2;

        FightHero fh = FightDataMgr.Instance.GetHero(heroId);
        Reward r = FightDataMgr.Instance.GetReward(nodeId);
        foreach (Reward.Condition cItem in r.ConditionData)
        {
            bool shouldGet = false;
            if(string.IsNullOrEmpty(cItem.Express))
            {
                shouldGet = true;   
            }
            else
            {
                System.Reflection.FieldInfo fi = fh.CreatureData.GetType().GetField(cItem.Express);
                string objVal = fi.GetValue(fh.CreatureData).ToString();
                shouldGet = objVal.Equals(cItem.Val);
            }
            if(shouldGet)
            {
                foreach (Item item in cItem.Rewards)
                {
                    fh.AddItem(item);
                }
            }
        }

        //move hero to targetNode
        fh.NowNodeId = nodeId;
        _stageConfig.GetNode(nodeId).IsPassed = true;
        
        EventSys.Instance.AddEvent(ViewEvent.GetRewardReturnToStage, fh, nodeId);

    }
}
