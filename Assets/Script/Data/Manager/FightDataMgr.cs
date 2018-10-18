using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightDataMgr {


    static FightDataMgr _instance;
    public static FightDataMgr Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new FightDataMgr();
            }
            return _instance;
        }
    }

    Dictionary<int, FightHero> _heros;
    Dictionary<int, Enemy> _enemies;
    Dictionary<int, Reward> _rewards;
    
    private FightDataMgr()
    {
        EventSys.Instance.AddHander(LogicEvent.CreateFightStageData, CreateNodes);
        EventSys.Instance.AddHander(LogicEvent.CreateFightHeroData, CreateFightHeros);
    }

    public void Init()
    {
        _heros = new Dictionary<int, FightHero>();
        _enemies = new Dictionary<int, Enemy>();
        
    }

    void Clear()
    {
        _heros.Clear();
        _enemies.Clear();
        _rewards.Clear();
        Init();
    }

    void CreateNodes(int eventId, object p1, object p2)
    {
        _enemies = new Dictionary<int, Enemy>();
        _rewards = new Dictionary<int, Reward>();
        //
        StageConfig stageConfig = (StageConfig)p1;
        foreach (StageLayer sl in stageConfig.Layers)
        {
            foreach (BaseStageNode node in sl.Nodes)
            {
                if (node is StageNodeFight)
                {
                    StageNodeFight nodeF = (StageNodeFight)node;
                    Enemy enemy = new Enemy(nodeF.EnemyId, nodeF.EnemyLv, nodeF.EnemyAi);
                    _enemies.Add(node.Id, enemy);
                }
                else if(node is StageNodeReward)
                {
                    StageNodeReward nodeR = (StageNodeReward)node;
                    Reward reward = new Reward(nodeR.RewardItemIds, nodeR.RewardItemNums, nodeR.Icon);
                    _rewards.Add(node.Id, reward);
                }
            }
        }
    }

    void CreateFightHeros(int id, object p1, object p2)
    {
        _heros = new Dictionary<int, FightHero>();
        foreach (Hero hero in PlayerDataMgr.Instance.Heros)
        {
            FightHero fHero = new FightHero(hero);
            _heros.Add(fHero.Id, fHero);
        }
    }

    public FightHero GetHero(int heroId)
    {
        FightHero result;
        _heros.TryGetValue(heroId, out result);
        return result;
    }

    public Enemy GetEnemy(int nodeId)
    {
        Enemy result;
        _enemies.TryGetValue(nodeId, out result);
        return result;
    }

    public Reward GetReward(int nodeId)
    {
        Reward result;
        _rewards.TryGetValue(nodeId, out result);
        return result;
    }
}
