using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Person {

    public int PetId { private set; get; }

    public int WeaponId { private set; get; }
    public int WeaponLv { private set; get; }

    public int EquipId { private set; get; }
    public int EquipLv { private set; get; }

    public List<int> Items;

}
