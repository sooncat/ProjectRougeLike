using System.Collections;
using System.Collections.Generic;
using com.initialworld.framework;
using UnityEngine;

public class EnemyData : CreatureData {

    public ENum<int> Ai;
    public List<ENum<int>> DropIds;

    public EnemyData(MonsterDataInfo dataInfo, int lv, int ai)
    {
        Id = dataInfo.Id;
        Name = dataInfo.Name;
        Description = dataInfo.Description;

        Lv = new ENum<int>(lv);
        Sex = new ENum<int>(dataInfo.Sex);
        Hp = new ENum<int>(dataInfo.Hp);
        HpMax = new ENum<int>(dataInfo.Hp);
        Mp = new ENum<int>(dataInfo.Mp);
        MpMax = new ENum<int>(dataInfo.Mp);
        Def = new ENum<int>(dataInfo.Def);
        Att = new ENum<int>(dataInfo.Att);
        Icon = dataInfo.Icon;
        Cg = dataInfo.Cg;

        Ai = new ENum<int>(ai);
        DropIds = new List<ENum<int>>();
        foreach (int dropId in dataInfo.DropIds)
        {
            DropIds.Add(new ENum<int>(dropId));
        }
        
    }
}
