using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature, ICreature
{
    
    public EnemyData Data;

    public CreatureData CreatureData
    {
        get { return Data; }
    }

    public Enemy(int configId, int lv, int ai)
    {
        Id = configId;
        MonsterDataInfo monsterTableDataInfo = (MonsterDataInfo)ConfigDataMgr.Instance.GetDataInfo<MonsterTableData>(Id);
        Data = new EnemyData(monsterTableDataInfo, lv, ai);
    }
}
