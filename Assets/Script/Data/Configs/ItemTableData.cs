using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemTableData : IDataConfig
{
    public List<ItemDataInfo> Data;

    public IList GetDataInfoList()
    {
        return Data;
    }
}
public class ItemDataInfo : BaseDataInfo
{

    public string Name;
    public string Description;

    public int Lv;
    public string Icon;
    public int Price;
    public int Type;
    public bool UsableInFight;
    public string Color;
    public int JobLimited;
    public string Command;
    
}
