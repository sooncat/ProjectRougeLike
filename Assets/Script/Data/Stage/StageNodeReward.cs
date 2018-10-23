using System.Collections;
using System.Collections.Generic;

public class StageNodeReward : BaseStageNode
{
    public struct ConditionConfig
    {
        public string ConditionExpress;
        public string Val;
        public Dictionary<int,int> Rewards;
    }

    public List<ConditionConfig> Rewards;

    public StageNodeReward(int id, int index, string name, string des, string icon)
        : base(id, index, name, des, icon)
    {
        Rewards = new List<ConditionConfig>();
    }

    public void AddData(string conditionExpress, string val,
        Dictionary<int, int> rewards)
    {
        ConditionConfig c = new ConditionConfig();
        c.ConditionExpress = conditionExpress;
        c.Val = val;
        c.Rewards = new Dictionary<int,int>();
        foreach (KeyValuePair<int, int> pair in rewards)
        {
            c.Rewards.Add(pair.Key, pair.Value);
        }
        Rewards.Add(c);
    }
}
