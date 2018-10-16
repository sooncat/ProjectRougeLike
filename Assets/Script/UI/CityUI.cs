using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityUI : BaseUI {

    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);
        Button btnLogin = rootNode.GetRef("ButtonFight").GetComponent<Button>();
        btnLogin.onClick.AddListener(OnBtnFightClicked);

        Button btnExit = rootNode.GetRef("ButtonExit").GetComponent<Button>();
        btnExit.onClick.AddListener(OnBtnExitClicked);
    }

    void OnBtnFightClicked()
    {
        FightStateParameter fsParameter = new FightStateParameter();
        fsParameter.nextType = "stage1";
        fsParameter.heros = new List<Hero>();

        for (int i = 1; i <= 3;i++ )
        {
            NetMessages.HeroServerData hsData = new NetMessages.HeroServerData();
            hsData.Id = i;
            hsData.Lv = 1;
            hsData.EquipId = 1;
            hsData.EquipLv = 1;
            hsData.PetId = 1;
            hsData.WeaponId = 1;
            hsData.WeaponLv = 1;

            Hero h = new Hero(hsData);
            fsParameter.heros.Add(h);
        }

        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(FightState), fsParameter);
    }

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(LoginState));
    }
}
