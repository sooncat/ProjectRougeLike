using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FightState : BaseGameState {

    string _stageName;
    StageConfig _stageConfig;

    Dictionary<int, Hero> _heros;
    Dictionary<int, Enemy> _enemies;

    public override void Enter(GameStateParameter parameter)
    {
        base.Enter(parameter);

        FightStateParameter fsParameter = (FightStateParameter)parameter;

        _stageName = fsParameter.nextType.ToString();
        _heros = new Dictionary<int, Hero>();
        foreach (Hero hero in fsParameter.heros)
        {
            _heros.Add(hero.Id, hero);
        }
    }

    protected override void OnUiLoaded(int id, object p1, object p2)
    {
        base.OnUiLoaded(id, p1, p2);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        string str = IOUtils.ReadFileString("Assets/StreamingAssets/StageConfig/" + _stageName + ".json");
        _stageConfig = JsonConvert.DeserializeObject<StageConfig>(str, settings);

        EventSys.Instance.AddEvent(LogicEvent.DrawFightUi, _stageConfig);

        CreateCreatures();

        OnAllPreLoaded();
    }

    void CreateCreatures()
    {
        _enemies = new Dictionary<int, Enemy>();
        foreach (StageLayer sl in _stageConfig.Layers)
        {
            foreach (BaseStageNode node in sl.Nodes)
            {
                if(node is StageNodeFight)
                {
                    StageNodeFight nodeF = (StageNodeFight)node;
                    Enemy enemy = new Enemy(nodeF.EnemyId, nodeF.EnemyLv, nodeF.EnemyAi);
                    _enemies.Add(nodeF.Id, enemy);
                    Debug.Log("enemy.name = " + enemy.CreatureData.Name);
                }
            }
        }
    }
}
