using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public class HeroData : CreatureData, ICloneable {

    public ENum<int> PetId { private set; get; }

    public ENum<int> WeaponId { private set; get; }
    public ENum<int> WeaponLv { private set; get; }

    public ENum<int> EquipId { private set; get; }
    public ENum<int> EquipLv { private set; get; }

    //public List<ENum<int>> Items;

    public string FightIcon;
    public Color TheColor;

    protected HeroData()
    {}

    public HeroData(HeroDataInfo dataInfo, int lv)
    {
        Id = dataInfo.Id;
        Name = dataInfo.Name;
        Description = dataInfo.Description;

        Lv = new ENum<int>(lv);
        Sex = new ENum<int>(dataInfo.Sex);
        Hp = new CENum<int>((int)PropertyType.Origin, dataInfo.Hp);
        Mp = new CENum<int>((int)PropertyType.Origin, dataInfo.Mp);
        Def = new CENum<int>((int)PropertyType.Origin, dataInfo.Def);
        Att = new CENum<int>((int)PropertyType.Origin, dataInfo.Att);

        //PetId = new ENum<int>(dataInfo.Pet);
        //WeaponId = new ENum<int>(dataInfo.Weapon);
        //WeaponLv = new ENum<int>(1);

        //EquipId = new ENum<int>(dataInfo.Equip);
        //EquipLv = new ENum<int>(1);
        Icon = dataInfo.Icon;
        Cg = dataInfo.Cg;
        FightIcon = dataInfo.FightIcon;
        ColorUtility.TryParseHtmlString(dataInfo.Color, out TheColor);

    }

    public object Clone()
    {
        HeroData result = new HeroData();
        result.Id = Id;
        result.Name = Name;
        result.Description = Description;

        result.Lv = new ENum<int>(Lv.Value);
        result.Sex = new ENum<int>(Sex.Value);
        result.Hp = new CENum<int>((int)PropertyType.Origin, Hp.Value);
        result.Mp = new CENum<int>((int)PropertyType.Origin, Mp.Value);
        result.Def = new CENum<int>((int)PropertyType.Origin, Def.Value);
        result.Att = new CENum<int>((int)PropertyType.Origin, Att.Value);
        
        result.Cg = Cg;
        result.FightIcon = FightIcon;
        result.TheColor = new Color(TheColor.r, TheColor.g, TheColor.b, TheColor.a);
        
        return result;
    }
}
