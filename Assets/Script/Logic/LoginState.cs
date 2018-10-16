using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginState : BaseGameState {

    public override void Enter(GameStateParameter parameter)
    {
        base.Enter(parameter);


        //此处数据从网络得到或者从本地读取
        NetMessages.PlayerData pd = new NetMessages.PlayerData();
        pd.Heros = new List<NetMessages.HeroServerData>();
        for (int i = 1; i <= 3; i++)
        {
            NetMessages.HeroServerData hsData = new NetMessages.HeroServerData();
            hsData.Id = i;
            hsData.Lv = 1;
            hsData.EquipId = 1;
            hsData.EquipLv = 1;
            hsData.PetId = 1;
            hsData.WeaponId = 1;
            hsData.WeaponLv = 1;
            pd.Heros.Add(hsData);
        }

        EventSys.Instance.AddEvent(NetEvent.CreatePlayerDatas, pd);
    }
}
