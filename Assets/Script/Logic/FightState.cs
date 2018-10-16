using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FightState : BaseGameState {

    string _stageName;
    StageConfig _stageConfig;

    public override void Enter(GameStateParameter parameter)
    {
        base.Enter(parameter);

        FightStateParameter fsParameter = (FightStateParameter)parameter;
        _stageName = fsParameter.nextType;
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
        EventSys.Instance.AddEvent(LogicEvent.DrawFightStageUi, _stageConfig);
        
        OnAllPreLoaded();
    }

    
}
