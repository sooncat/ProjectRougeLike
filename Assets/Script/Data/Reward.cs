using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 奖励，可以用于战斗节点/战斗后奖励
/// </summary>
public class Reward {

    public int Id;
    public string Name;
    public string Description;

    public string Icon;

    public struct Condition : IComparable<Condition>
    {
        public string Express;
        public string Val;
        public List<Item> Rewards;

        public int CompareTo(Condition obj)
        {
            return String.Compare(Express, obj.Express, StringComparison.Ordinal);
        }
    }

    public List<Condition> ConditionData;

    public Reward(StageNodeReward node)
    {
        ConditionData = new List<Condition>();
        foreach (StageNodeReward.ConditionConfig c in node.Rewards)
        {
            Condition thisCondition = new Condition();
            thisCondition.Express = c.ConditionExpress;
            thisCondition.Val = c.Val;
            thisCondition.Rewards = new List<Item>(); ;
            foreach (KeyValuePair<int, int> pair in c.Rewards)
            {
                Item item = new Item(pair.Key, pair.Value);
                thisCondition.Rewards.Add(item);
            }
            ConditionData.Add(thisCondition);
        }
        Icon = node.Icon;
    }

}
