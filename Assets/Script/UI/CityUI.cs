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
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(FightState), "stage1");
    }

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(LoginState));
    }
}
