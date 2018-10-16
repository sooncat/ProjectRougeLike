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

    //fight property
    public int Sex;
    public int Hp;
    public int Mp;
    public int Def;
    public int Att;

}
