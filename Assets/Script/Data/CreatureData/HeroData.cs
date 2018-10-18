using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.initialworld.framework;

public class HeroData : CreatureData {

    public ENum<int> PetId { private set; get; }

    public ENum<int> WeaponId { private set; get; }
    public ENum<int> WeaponLv { private set; get; }

    public ENum<int> EquipId { private set; get; }
    public ENum<int> EquipLv { private set; get; }

    public List<ENum<int>> Items;

    public string FightIcon;
    public Color TheColor;
    
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
        Cg = dataInfo.Cg;
        FightIcon = dataInfo.FightIcon;
        ColorUtility.TryParseHtmlString(dataInfo.Color, out TheColor);

    }

}
