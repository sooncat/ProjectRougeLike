using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 安全屋节点，可以容纳多个英雄，允许英雄交换道具
/// </summary>
public class StageNodeSafe : BaseStageNode {

    public StageNodeSafe(int id, int index, string name, string des, string icon)
        : base(id, index, name, des, icon)
    {

    }
}
