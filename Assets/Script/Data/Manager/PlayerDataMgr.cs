using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerDataMgr {

    private static PlayerDataMgr _instance;
    public static PlayerDataMgr Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new PlayerDataMgr();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 玩家的英雄
    /// </summary>
    public List<Hero> Heros;
    
    /// <summary>
    /// 玩家的道具
    /// </summary>
    public List<Item> Items;
    
    /// <summary>
    /// 成就
    /// </summary>
    public List<Archievement> Rewards;

    private PlayerDataMgr()
    {}

    public void Init()
    {
        Heros = new List<Hero>();
        Items = new List<Item>();
        Rewards = new List<Archievement>();

        EventSys.Instance.AddHander(NetEvent.CreatePlayerDatas, OnCreatePlayerDatas);
    }

    void OnCreatePlayerDatas(object p1, object p2)
    {
        NetMessages.PlayerData pd = (NetMessages.PlayerData)p1;

        foreach (NetMessages.HeroServerData hsData in pd.Heros)
        {
            Hero h = new Hero(hsData);
            Heros.Add(h);
        }
    }

    public Hero GetHero(int id)
    {
        foreach (Hero hero in Heros)
        {
            if (hero.Id == id)
                return hero;
        }
        return null;
    }
}
