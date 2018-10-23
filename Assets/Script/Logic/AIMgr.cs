using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMgr {

    private static AIMgr _instance;
    public static AIMgr Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new AIMgr();
            }
            return _instance;
        }
    }

    private AIMgr()
    {
        EventSys.Instance.AddHander(LogicEvent.FightStartAi, StartAI);
    }

    public void Init()
    {
        
    }

    void StartAI(object p1, object p2)
    {
        CatDebug.LogFunc();

        Dictionary<int, FightHero> heroes = (Dictionary<int, FightHero>)p1;
        List<Enemy> enemies = (List<Enemy>)p2;

        foreach (Enemy enemy in enemies)
        {
            if(enemy.CreatureData.Hp.Value > 0)
            {
                //目前AI只有一种行为就是普通攻击
                FightHero targetHero = SelectHero(heroes);
                if(targetHero != null)
                {
                    EventSys.Instance.AddEventNow(AiInputEvent.Attack, enemy, targetHero);
                }    
            }
        }

        EventSys.Instance.AddEvent(AiInputEvent.AiActionEnd);
    }

    /// <summary>
    /// 随机选择一个英雄
    /// </summary>
    /// <param name="heroes"></param>
    /// <returns></returns>
    FightHero SelectHero(Dictionary<int, FightHero> heroes)
    {
        //
        List<FightHero> fHeros = new List<FightHero>();
        foreach (KeyValuePair<int, FightHero> pair in heroes)
        {
            fHeros.Add(pair.Value);
        }

        //
        System.Random random = new System.Random();
        int randomNum = random.Next(0, fHeros.Count);

        //
        for (int i = 0; i < fHeros.Count;i++ )
        {
            int index = (randomNum + i) % fHeros.Count;
            FightHero fHero = fHeros[index];
            if (fHero.CreatureData.Hp.Value > 0)
            {
                return fHero;
            }
        }

        Debug.LogError("All Heros Died");
        return null;
    }
}
