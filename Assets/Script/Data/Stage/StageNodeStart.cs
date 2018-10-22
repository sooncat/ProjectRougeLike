using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 起点的Node，用于玩家选择出场人物。
/// 必须配置在第0层。
/// </summary>
public class StageNodeStart : BaseStageNode {

    public StageNodeStart(int id, int index, string name, string des, string icon)
        : base(id,index,name,des,icon)
    {
        
    }

}
