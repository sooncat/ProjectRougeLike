using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public class Hero : Creature, ICreature {

    protected HeroData Data;

    public CreatureData CreatureData
    {
        get { return Data; }
    }

    protected Hero()
    {}

    public Hero(NetMessages.HeroServerData hsData)
    {
        Id = hsData.Id;
        HeroDataInfo heroTableDataInfo = (HeroDataInfo)ConfigDataMgr.Instance.GetDataInfo<HeroTableData>(Id);
        Data = new HeroData(heroTableDataInfo, hsData.Lv);
    }

    
}
