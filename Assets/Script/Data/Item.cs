using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

/// <summary>
/// 道具
/// </summary>
public class Item {

    public int Id;
    public string Name;
    public string Description;

    public int Type;
    public ENum<int> Count;
    public string Command;
    public string Icon;
    public ENum<int> Price;
    public ENum<int> Lv; 

    public Item(int id, int count)
    {
        Id = id;
        Count = new ENum<int>(count);

        ItemDataInfo dataInfo = (ItemDataInfo)ConfigDataMgr.Instance.GetDataInfo<ItemTableData>(Id);
        Name = dataInfo.Name;
        Description = dataInfo.Description;
        Type = dataInfo.Type;
        Command = dataInfo.Command;
        Icon = dataInfo.Icon;
        Price.Value = dataInfo.Price;
        Lv.Value = dataInfo.Lv;
    }
}
