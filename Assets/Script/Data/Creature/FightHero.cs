using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public class FightHero : Hero{

    /// <summary>
    /// 身上携带的道具
    /// </summary>
    public List<Item> Items;

    public FightHero(Hero hero)
    {
        Data = ((HeroData)hero.CreatureData).Clone() as HeroData;
        Id = hero.Id;
        Items = new List<Item>();
    }
}
