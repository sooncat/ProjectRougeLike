using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    void OnExitEvent(int id, object p1, object p2)
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(CityState));
    }

    void OnFightReadyEvent(int id, object p1, object p2)
    {
        _fState = FSubState.Mapping;
        EventSys.Instance.AddEventNow(LogicEvent.CreateFightHeroData, _selectedHeros);
        EventSys.Instance.AddEvent(ViewEvent.FightSubStateMapping, FightDataMgr.Instance.GetHeros());
    }

    void OnClickFightNodeEvent(int id, object p1, object p2)
    {
        
    }

    protected override void OnUiLoaded(int id, object p1, object p2)
    {
        base.OnUiLoaded(id, p1, p2);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        string str = IOUtils.ReadFileString(GameConstants.StageConfigPath + _stageName + GameConstants.StageConfigTail);
        _stageConfig = JsonConvert.DeserializeObject<StageConfig>(str, settings);

        EventSys.Instance.AddEvent(LogicEvent.CreateFightStageData, _stageConfig);
        //EventSys.Instance.AddEvent(LogicEvent.CreateFightHeroData);
        EventSys.Instance.AddEvent(ViewEvent.CreateStageView, _stageConfig);
        
        OnAllPreLoaded();
    }

    void OnFightDragOnNode(int id, object p1, object p2)
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

                break;
        }
    }

    void OnFightDragOnHero(int id, object p1, object p2)
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
                    EventSys.Instance.AddEvent(ViewEvent.ReplaceHeroStartNode, newHero.CreatureData, new int[] { targetHeroId, nodeId });
                }
                break;
            case FSubState.Mapping:
                
                break;
        }
    }
}
