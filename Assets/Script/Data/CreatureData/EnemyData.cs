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
        Hp = new CENum<int>((int)PropertyType.Origin, dataInfo.Hp);
        HpMax = new CENum<int>((int)PropertyType.Origin, dataInfo.Hp);
        Mp = new CENum<int>((int)PropertyType.Origin, dataInfo.Mp);
        MpMax = new CENum<int>((int)PropertyType.Origin, dataInfo.Mp);
        Def = new CENum<int>((int)PropertyType.Origin, dataInfo.Def);
        Att = new CENum<int>((int)PropertyType.Origin, dataInfo.Att);
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
