using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTableData : IDataConfig
{
    public List<HeroDataInfo> Data;

    public IList GetDataInfoList()
    {
        return Data;
    }
}

public class HeroDataInfo : BaseDataInfo {

    public string Name;
    public string Description;
    public int Job;

    //fight property
    public int Sex;
    public int Hp;
    public int Mp;
    public int Def;
    public int Att;

    public string Icon;
    public string Cg;
    public string Color;
    public string FightIcon;
    
}
