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

    //Dictionary<int, FightHero> _heros;


	public FightProgress()
	{
        EventSys.Instance.AddEvent(LogicEvent.StartFightRound, StartRound);

	    EventSys.Instance.AddHander(InputEvent.FightAttack, OnUseAttack);
        EventSys.Instance.AddHander(InputEvent.FightItem, OnUseItem);
        EventSys.Instance.AddHander(InputEvent.FightSelectHero, OnSelectHero);
	}

    void OnUseAttack(int id, object p1, object p2)
    {
        int heroId = (int)p1;
        int enemyId = (int)p2;
        FightHero hero = FightDataMgr.Instance.GetHero(heroId);
        Enemy enemy = FightDataMgr.Instance.GetEnemy(enemyId);

        int damage = hero.CreatureData.Att.Value - enemy.CreatureData.Def.Value;
        enemy.CreatureData.Hp.Value -= damage;

        if(enemy.CreatureData.Hp.Value <= 0)
        {
            
        }
    }

    void OnUseItem(int id, object p1, object p2)
    {
        
    }

    void OnSelectHero(int id, object p1, object p2)
    {
        int heroId = (int)p1;
    }

    void StartRound(int id, object p1, object p2)
    {
        _round = 0;
        NextRound();

        EventSys.Instance.AddEvent(ViewEvent.CreateFightView, FightDataMgr.Instance.GetHeros(), FightDataMgr.Instance.GetEnemies());
        EventSys.Instance.AddEvent(ViewEvent.SetSelectedHero, FightDataMgr.Instance.GetHero(_nowHeroId));
    }

    /// <summary>
    /// 进入下一个回合
    /// </summary>
    void NextRound()
    {
        bool isFirst = true;
        Dictionary<int, FightHero> heros = FightDataMgr.Instance.GetHeros();
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            if(isFirst)
            {
                _nowHeroId = pair.Value.Id;
                isFirst = false;
            }
            pair.Value.IsActioned = false;
        }
        Dictionary<int, Enemy> enemies = FightDataMgr.Instance.GetEnemies();
        foreach (KeyValuePair<int, Enemy> pair in enemies)
        {
            pair.Value.IsActioned = false;
        }
        _round++;
    }
}
