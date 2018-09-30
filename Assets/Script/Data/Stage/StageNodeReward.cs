using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNodeReward : BaseStageNode
{
    public int RewardItemId;
    public int RewardItemNum;

    public StageNodeReward(int id, int index, string name, string des, string icon)
        : base(id, index, name, des, icon)
    {

    }

    public void SetData(int rewardItemId, int rewardItemNum)
    {
        RewardItemId = rewardItemId;
        RewardItemNum = rewardItemNum;
    }
}
