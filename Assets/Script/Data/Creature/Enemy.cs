using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Enemy : Creature, ICreature
{
    
    EnemyData Data;

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
