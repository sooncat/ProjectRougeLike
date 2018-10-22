using System.Collections;
using System.Collections.Generic;


public class StageNodeFight : BaseStageNode
{
    public int EnemyId;
    public int EnemyLv;
    public int EnemyAi;

    public StageNodeFight(int id, int index, string name, string des, string icon)
        : base(id, index, name, des, icon)
    {

    }

    public void SetData(int enemyId, int enemyLv, int enemyAi)
    {
        EnemyId = enemyId;
        EnemyLv = enemyLv;
        EnemyAi = enemyAi;
    }
}
