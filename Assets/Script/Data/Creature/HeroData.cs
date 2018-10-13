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

    public HeroData()
    {
        
    }

}
