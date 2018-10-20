using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 回合战斗类
/// </summary>
public class FightProgress {

    /// <summary>
    /// 战斗回合计数
    /// </summary>
    int _round;

    /// <summary>
    /// 当前选中英雄
    /// </summary>
    int _nowHeroId;

    /// <summary>
    /// 当前进行战斗的英雄
    /// </summary>
    Dictionary<int, FightHero> _heros;

    /// <summary>
    /// 当前进行战斗的敌人
    /// </summary>
    List<Enemy> _enemies;

    /// <summary>
    /// 当前节点ID
    /// </summary>
    int _nowNodeId;

    StageConfig _stageConfig;
    
    public FightProgress(StageConfig stageConfig)
	{
        _stageConfig = stageConfig;

        EventSys.Instance.AddHander(LogicEvent.StartFightRound, StartRound);
        EventSys.Instance.AddHander(InputEvent.FightAttack, OnHeroAttack);
        EventSys.Instance.AddHander(InputEvent.FightItem, OnHeroUseItem);
        EventSys.Instance.AddHander(InputEvent.FightSelectHero, OnSelectHero);
	}

    /// <summary>
    /// 当前选中英雄的普通攻击
    /// </summary>
    /// <param name="id"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnHeroAttack(object p1, object p2)
    {
        FightHero hero = _heros[_nowHeroId];
        Enemy enemy = _enemies[0];

        int damage = hero.CreatureData.Att.Value - enemy.CreatureData.Def.Value;
        damage = System.Math.Max(1, damage);
        enemy.CreatureData.Hp.Value -= damage;

        if(damage > 0)
        {
            EventSys.Instance.AddEvent(ViewEvent.FightEnemyHurt, damage);
            EventSys.Instance.AddEvent(ViewEvent.FightUpdateEnemyState, enemy);
        }

        if (enemy.CreatureData.Hp.Value <= 0)
        {
            EventSys.Instance.AddEvent(ViewEvent.FightShowWin);
            _stageConfig.GetNode(_nowNodeId).IsPassed = true;
            foreach (KeyValuePair<int, FightHero> pair in _heros)
            {
                pair.Value.NowNodeId = _nowNodeId;
            }
            TimerUtils.Instance.StartTimer(1, () =>
            {
                EventSys.Instance.AddEvent(ViewEvent.FightReturnToStage, _heros, _nowNodeId);
            });
        }
    }

    void OnHeroUseItem( object p1, object p2)
    {
        
    }

    void OnSelectHero( object p1, object p2)
    {
        _nowHeroId = (int)p1;
        EventSys.Instance.AddEvent(ViewEvent.SetSelectedHero, FightDataMgr.Instance.GetHero(_nowHeroId));
    }

    void StartRound(object p1, object p2)
    {
        int[] heroIds = (int[])p1;
        _heros = new Dictionary<int, FightHero>();
        foreach (int heroId in heroIds)
        {
            _heros.Add(heroId, FightDataMgr.Instance.GetHero(heroId));    
        }

        _nowNodeId = (int)p2;
        _enemies = new List<Enemy>();
        _enemies.Add(FightDataMgr.Instance.GetEnemy(_nowNodeId));

        EventSys.Instance.AddEvent(ViewEvent.CreateFightView, _heros, _enemies);
        
        _round = 0;
        NextRound();
    }

    /// <summary>
    /// 进入下一个回合
    /// </summary>
    void NextRound()
    {
        bool isFirst = true;
        
        foreach (KeyValuePair<int, FightHero> pair in _heros)
        {
            if(isFirst)
            {
                _nowHeroId = pair.Value.Id;
                isFirst = false;
            }
            pair.Value.IsActioned = false;
        }
        
        foreach (Enemy e in _enemies)
        {
            e.IsActioned = false;
        }
        _round++;

        EventSys.Instance.AddEvent(ViewEvent.FightUpdateRound, _round, _heros[_nowHeroId]);
    }
}
