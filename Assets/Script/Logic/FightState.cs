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
        EventSys.Instance.AddHander(InputEvent.FightDrag, OnFightDrag);
    }

    void OnExitEvent(int id, object p1, object p2)
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(CityState));
    }

    void OnFightReadyEvent(int id, object p1, object p2)
    {
        _fState = FSubState.Mapping;
        EventSys.Instance.AddEvent(ViewEvent.FightSubStateMapping);
        EventSys.Instance.AddEvent(LogicEvent.CreateFightHeroData, _selectedHeros);
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

    void OnFightDrag(int id, object p1, object p2)
    {
        int heroId = (int)p1;
        int targetNodeId = (int)p2;

        CatDebug.LogFunc(" p1 = " + heroId + ", p2 = " + targetNodeId);

        switch (_fState)
        {
            case FSubState.SelectHero:
                Hero h = PlayerDataMgr.Instance.GetHero(heroId);
                if(h != null)
                {
                    if(_selectedHeros.ContainsKey(targetNodeId))
                    {
                        _selectedHeros[targetNodeId] = h.Id;
                        EventSys.Instance.AddEvent(ViewEvent.ReplaceHeroStartNode, h.CreatureData, targetNodeId);
                    }
                    else
                    {
                        _selectedHeros.Add(targetNodeId, h.Id);
                        EventSys.Instance.AddEvent(ViewEvent.CreateHeroStartNode, h.CreatureData, targetNodeId);        
                    }
                }
                
                break;
            case FSubState.Mapping:

                break;
        }
    }
}
