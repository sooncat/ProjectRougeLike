using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTableData : IDataConfig
{
    public List<MonsterDataInfo> Data;

    public IList GetDataInfoList()
    {
        return Data;
    }
}

public class MonsterDataInfo : BaseDataInfo
{

    public string Name;
    public string Description;

    //fight property
    public int Sex;
    public int Hp;
    public int Mp;
    public int Def;
    public int Att;
    public string Icon;

    public List<int> DropIds;
}
