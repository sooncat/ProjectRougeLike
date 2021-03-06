﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityView : BaseView {

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
        
        EventSys.Instance.AddEvent(InputEvent.CityStartFight, fsParameter);
    }

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.CityExit);
    }
}
