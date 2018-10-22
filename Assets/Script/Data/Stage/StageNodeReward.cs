using System.Collections;
using System.Collections.Generic;


public class StageNodeReward : BaseStageNode
{
    public int[] RewardItemIds;
    public int[] RewardItemNums;

    public StageNodeReward(int id, int index, string name, string des, string icon)
        : base(id, index, name, des, icon)
    {

    }

    public void SetData(int[] rewardItemId, int[] rewardItemNum)
    {
        RewardItemIds = rewardItemId;
        RewardItemNums = rewardItemNum;
    }
}
