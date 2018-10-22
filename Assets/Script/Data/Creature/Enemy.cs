using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Enemy : Creature, ICreature
{
    /// <summary>
    /// 实例的ID，避免在同一个节点中出现两个相同的敌人？
    /// </summary>
    public int InstanceId;

    EnemyData Data;

    /// <summary>
    /// 是否行动过
    /// </summary>
    public bool IsActioned;

    public CreatureData CreatureData
    {
        get { return Data; }
    }



    public Enemy(int configId, int lv, int ai, int index)
    {
        Id = configId;
        MonsterDataInfo monsterTableDataInfo = (MonsterDataInfo)ConfigDataMgr.Instance.GetDataInfo<MonsterTableData>(Id);
        Data = new EnemyData(monsterTableDataInfo, lv, ai);
        InstanceId = Id * 100 + index;
    }

    

}
