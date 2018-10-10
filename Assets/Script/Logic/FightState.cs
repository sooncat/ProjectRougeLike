using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FightState : BaseGameState {

    string _stageName;
    StageConfig _stageConfig;

    public override void Enter(object parameter)
    {
        base.Enter(parameter);
        _stageName = parameter.ToString();
    }

    protected override void OnUiLoaded(int id, object p1, object p2)
    {
        base.OnUiLoaded(id, p1, p2);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        byte[] b = Util.ReadFile("Assets/StreamingAssets/StageConfig/" + _stageName + ".json");
        string str = System.Text.Encoding.UTF8.GetString(b);
        _stageConfig = JsonConvert.DeserializeObject<StageConfig>(str, settings);

        EventSys.Instance.AddEvent(LogicEvent.DrawFightUi, _stageConfig);

        OnAllPreLoaded();
    }
}
