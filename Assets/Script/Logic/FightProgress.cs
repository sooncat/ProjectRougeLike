using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    enum RoundTurn
    {
        Hero,
        Enemy
    }

    RoundTurn _nowTurn;
    
    public FightProgress(StageConfig stageConfig)
	{
        _stageConfig = stageConfig;

        EventSys.Instance.AddHander(LogicEvent.StartFightRound, StartRound);

        EventSys.Instance.AddHander(InputEvent.FightAttack, OnHeroAttack);
        EventSys.Instance.AddHander(InputEvent.FightItem, OnHeroUseItem);
        EventSys.Instance.AddHander(InputEvent.FightSelectHero, OnSelectHero);

        EventSys.Instance.AddHander(AiInputEvent.Attack, OnAiAttack);
        EventSys.Instance.AddHander(AiInputEvent.AiActionEnd, OnAiEnd);

	}

    public void Clear()
    {
        _heros = new Dictionary<int, FightHero>();
        _enemies = new List<Enemy>();
        EventSys.Instance.RemoveHander(this);
    }

    void OnAiAttack(object p1, object p2)
    {
        Enemy enemy = (Enemy)p1;
        FightHero hero = (FightHero)p2;

        int damage = enemy.CreatureData.Att.Value - hero.CreatureData.Def.Value;
        damage = System.Math.Max(1, damage);

        hero.CreatureData.Hp.Value -= damage;
        if(hero.CreatureData.Hp.Value < 0)
        {
            hero.CreatureData.Hp.Value = 0;
        }

        EventSys.Instance.AddEvent(ViewEvent.FightEnemyAttack, hero, damage);
        //EventSys.Instance.AddEvent(ViewEvent.FightUpdateHeroState, _heros);//单体攻击的话没必要更新所有，群体攻击可以理解为多个单体攻击
    }

    void OnAiEnd(object p1, object p2)
    {
        bool isAllHeroDied = true;
        foreach (var pair in _heros)
        {
            if(pair.Value.CreatureData.Hp.Value > 0)
            {
                isAllHeroDied = false;
                break;
            }
        }
        if(isAllHeroDied)
        {
            EventSys.Instance.AddEvent(ViewEvent.FightShowLose, _heros);
            TimerUtils.Instance.StartTimer(1, () =>
            {
                EventSys.Instance.AddEvent(ViewEvent.FightLoseReturnToStage, _heros);
                EventSys.Instance.AddEvent(LogicEvent.FightLoseReturnToStage);
            });
        }
        else
        {
            NextRound();
        }
    }

    /// <summary>
    /// 当前选中英雄的普通攻击
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    void OnHeroAttack(object p1, object p2)
    {
        FightHero hero = _heros[_nowHeroId];
        hero.IsActioned = true;

        Enemy enemy = _enemies[0];

        int damage = hero.CreatureData.Att.Value - enemy.CreatureData.Def.Value;
        damage = System.Math.Max(1, damage);
        enemy.CreatureData.Hp.Value -= damage;
        if (enemy.CreatureData.Hp.Value <= 0)
        {
            enemy.CreatureData.Hp.Value = 0;
        }
        EventSys.Instance.AddEvent(ViewEvent.FightHeroAttack, damage);
        EventSys.Instance.AddEvent(ViewEvent.FightUpdateEnemyState, enemy);
        
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
                EventSys.Instance.AddEvent(ViewEvent.FightWinReturnToStage, _heros, _nowNodeId);
            });
        }
        else
        {
            CheckChangeTurnToEnemy();    
        }
        
    }

    /// <summary>
    /// 当玩家行动完毕后，检查是否需要切换双方主动权
    /// </summary>
    void CheckChangeTurnToEnemy()
    {
        bool isAllActioned = true;
        foreach (KeyValuePair<int, FightHero> pair in _heros)
        {
            if(!pair.Value.IsActioned)
            {
                isAllActioned = false;
                _nowHeroId = pair.Value.Id;
                FightHero fh = _heros[_nowHeroId];
                EventSys.Instance.AddEvent(ViewEvent.SetSelectedHero, fh);
                break;
            }
        }
        
        if(isAllActioned)
        {
            _nowTurn = RoundTurn.Enemy;
            EventSys.Instance.AddEvent(ViewEvent.FightChangeTurn, false);
            EventSys.Instance.AddEvent(LogicEvent.FightStartAi, _heros, _enemies);
        }
    }

    void OnHeroUseItem( object p1, object p2)
    {
        
    }

    void OnSelectHero( object p1, object p2)
    {
        _nowHeroId = (int)p1;
        EventSys.Instance.AddEvent(ViewEvent.SetSelectedHero, _heros[_nowHeroId]);
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
        _nowTurn = RoundTurn.Hero;

        EventSys.Instance.AddEvent(ViewEvent.FightUpdateRound, _round, _heros[_nowHeroId]);
        EventSys.Instance.AddEvent(ViewEvent.FightChangeTurn, true);
    }
}
