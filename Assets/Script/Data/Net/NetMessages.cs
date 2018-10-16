using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMessages {

	
    public class PlayerData
    {
        public List<HeroServerData> Heros;
        public List<ItemServerData> Items;
        
    }

    public class HeroServerData
    {
        public int Id;
        public int Lv;
        public int PetId;
        public int WeaponId;
        public int WeaponLv;
        public int EquipId;
        public int EquipLv;
    }

    public class ItemServerData
    {
        public int Id;
        public int Count;
    }
}
