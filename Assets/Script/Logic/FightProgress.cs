using System.Collections;
using System.Collections.Generic;
using com.initialworld.framework;
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
        EventSys.Instance.AddHander(InputEvent.FightSelectHero, OnSelectHero);
        EventSys.Instance.AddHander(InputEvent.FightUseItemToEnemy, OnUseItemToEnemy);
        EventSys.Instance.AddHander(InputEvent.FightUseItemToHero, OnUseItemToHero);
        EventSys.Instance.AddHander(InputEvent.FightWinConfirm, OnWinConfirm);
        EventSys.Instance.AddHander(InputEvent.FightLoseConfirm, OnLoseConfirm);
        EventSys.Instance.AddHander(InputEvent.FightItemClicked, OnItemClicked);

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
            EnemyBeHurtDead(enemy);
        }
        else
        {
            CheckChangeTurnToEnemy();
        }
    }

    void EnemyBeHurtDead(Enemy enemy)
    {
        List<ENum<int>> itemIds = ((EnemyData)enemy.CreatureData).DropIds;
        List<Item> items = new List<Item>();
        foreach (ENum<int> id in itemIds)
        {
            Item item = new Item(id.Value, 1);
            FightDataMgr.Instance.GetHero(_nowHeroId).AddItem(item);
            items.Add(item);
        }
        EventSys.Instance.AddEvent(ViewEvent.FightShowWin, items);
        
        _stageConfig.GetNode(_nowNodeId).IsPassed = true;
        foreach (KeyValuePair<int, FightHero> pair in _heros)
        {
            pair.Value.NowNodeId = _nowNodeId;
        }
    }

    void OnWinConfirm(object p1, object p2)
    {
        EventSys.Instance.AddEvent(ViewEvent.FightWinReturnToStage, _heros, _nowNodeId);
        EventSys.Instance.AddEvent(LogicEvent.FightWinReturnToStage, _nowNodeId);
    }

    void OnLoseConfirm(object p1, object p2)
    {
        EventSys.Instance.AddEvent(ViewEvent.FightLoseReturnToStage, _heros);
        EventSys.Instance.AddEvent(LogicEvent.FightLoseReturnToStage);
    }

    /// <summary>
    /// 当玩家行动完毕后，检查是否需要切换双方主动权
    /// </summary>
    void CheckChangeTurnToEnemy()
    {
        bool isAllActioned = true;
        foreach (KeyValuePair<int, FightHero> pair in _heros)
        {
            if(!pair.Value.IsActioned && pair.Value.CreatureData.Hp.Value > 0)
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

    void OnUseItemToEnemy(object p1, object p2)
    {
        int enemyInstanceId = (int)p1;
        int itemId = (int)p2;

        Enemy enemy = FightDataMgr.Instance.GetEnemyByInstanceId(enemyInstanceId);
        FightHero fh = FightDataMgr.Instance.GetHero(_nowHeroId);
        Item item = fh.Items[itemId];


        if(item.Id == 4)
        {
            int damage = 1000;
            enemy.CreatureData.Hp.Value -= damage;
            if (enemy.CreatureData.Hp.Value <= 0)
            {
                enemy.CreatureData.Hp.Value = 0;
            }
            EventSys.Instance.AddEvent(ViewEvent.FightHeroAttack, damage);
            EventSys.Instance.AddEvent(ViewEvent.FightUpdateEnemyState, enemy);

            if (enemy.CreatureData.Hp.Value <= 0)
            {
                EnemyBeHurtDead(enemy);
            }
            else
            {
                CheckChangeTurnToEnemy();
            }
            fh.DelItem(item.Id, 1);
            EventSys.Instance.AddEvent(ViewEvent.FightUpdateHeroState, fh);

            fh.IsActioned = true;
            CheckChangeTurnToEnemy();
        }
        else
        {
            EventSys.Instance.AddEvent(ViewEvent.FightShowTipNotSupportYet);
        }
        
    }

    void OnUseItemToHero(object p1, object p2)
    {
        int targetHeroId = (int)p1;
        int itemId = (int)p2;

        CatDebug.LogFunc("targetHeroId = " + targetHeroId + ", itemId = " + itemId);

        FightHero originHero = FightDataMgr.Instance.GetHero(_nowHeroId);
        FightHero targetHero = FightDataMgr.Instance.GetHero(targetHeroId);
        Item item = targetHero.Items[itemId];

        if(item.Id == 1)
        {
            int nowHp = targetHero.CreatureData.Hp.Value;
            if (nowHp >= 0)
            {
                targetHero.CreatureData.Hp.Value = System.Math.Min(nowHp+100, targetHero.CreatureData.HpMax.Value);
                originHero.DelItem(item.Id, 1);
                EventSys.Instance.AddEvent(ViewEvent.FightUpdateAllHeroState, _heros);
                EventSys.Instance.AddEvent(ViewEvent.FightHeroHpSupply, targetHero.Id, 100);

                originHero.IsActioned = true;
                CheckChangeTurnToEnemy();
            }
        }
        else if(item.Id == 2)
        {
            int nowMp = targetHero.CreatureData.Mp.Value;
            targetHero.CreatureData.Mp.Value = System.Math.Min(nowMp + 100, targetHero.CreatureData.MpMax.Value);
            originHero.DelItem(item.Id, 1);
            EventSys.Instance.AddEvent(ViewEvent.FightUpdateAllHeroState, _heros);
            EventSys.Instance.AddEvent(ViewEvent.FightHeroMpSupply, targetHero.Id, 100);

            originHero.IsActioned = true;
            CheckChangeTurnToEnemy();
        }
        else
        {
            EventSys.Instance.AddEvent(ViewEvent.FightShowTipNotSupportYet);
        }
        
    }

    void OnItemClicked(object p1, object p2)
    {
        int itemId = (int)p1;
        ItemDataInfo itemInfo = (ItemDataInfo)ConfigDataMgr.Instance.GetDataInfo<ItemTableData>(itemId);

        EventSys.Instance.AddEvent(ViewEvent.FightShowItemDes, itemInfo.Description);
    }
}
