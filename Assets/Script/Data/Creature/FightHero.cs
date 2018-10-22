using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public class FightHero : Hero{

    /// <summary>
    /// 身上携带的道具
    /// </summary>
    public Dictionary<int, Item> Items;

    int _nowNodeId;
    public int NowNodeId
    {
        get
        {
            return _nowNodeId;
        }
        set
        {
            if(_nowNodeId != value)
            {
                _lastNodeId = _nowNodeId;
                _nowNodeId = value;
            }
        }
    }
    int _lastNodeId;
    public int LastNodeId
    {
        get { return _lastNodeId; }
    }

    /// <summary>
    /// 是否行动过
    /// </summary>
    public bool IsActioned;

    public FightHero(Hero hero)
    {
        Data = ((HeroData)hero.CreatureData).Clone() as HeroData;
        Id = hero.Id;
        Items = new Dictionary<int, Item>();
        Data.HpMax = new ENum<int>(Data.Hp.Value);
        Data.MpMax = new ENum<int>(Data.Mp.Value);

        _lastNodeId = -1;
    }

    public void AddItem(Item item)
    {
        if(Items.ContainsKey(item.Id))
        {
            Items[item.Id].Count.Value += item.Count.Value;
        }
        else
        {
            Items.Add(item.Id, item);
        }
    }

    public void DelItem(int itemId, int num)
    {
        if(Items.ContainsKey(itemId))
        {
            if(Items[itemId].Count.Value >= num)
            {
                Items[itemId].Count.Value -= num;
                if(Items[itemId].Count.Value==0)
                {
                    Items.Remove(itemId);
                }
                return;
            }
            else
            {
                throw new Exception("Don't have enough Item id="+itemId+", num="+num+", owned="+Items[itemId].Count.Value);
            }
        }
        throw new Exception("Don't have Item id=" + itemId);
    }
}
