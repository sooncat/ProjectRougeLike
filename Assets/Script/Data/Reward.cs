using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 奖励，可以用于战斗节点/战斗后奖励
/// </summary>
public class Reward {

    public int Id;
    public string Name;
    public string Description;

    public List<Item> Data;
    public string Icon;

    public Reward(int[] itemIds, int[] itemNums, string icon = null)
    {
        Data = new List<Item>();
        for (int i = 0; i < itemIds.Length;i++ )
        {
            Item item = new Item(itemIds[i], itemNums[i]);
            Data.Add(item);
        }
        Icon = icon;
    }

}
