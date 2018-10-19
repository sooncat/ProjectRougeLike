using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public class FightHero : Hero{

    /// <summary>
    /// 身上携带的道具
    /// </summary>
    public List<Item> Items;

    public int NowNodeId;

    /// <summary>
    /// 是否行动过
    /// </summary>
    public bool IsActioned;

    public FightHero(Hero hero)
    {
        Data = ((HeroData)hero.CreatureData).Clone() as HeroData;
        Id = hero.Id;
        Items = new List<Item>();
        Data.HpMax = new ENum<int>(Data.Hp.Value);
        Data.MpMax = new ENum<int>(Data.Mp.Value);
    }
}
