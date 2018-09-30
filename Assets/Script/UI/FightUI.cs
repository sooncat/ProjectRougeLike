using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : BaseUI
{
    public override void InitUI(UINode rootNode)
    {
        Button btnExit = rootNode.GetRef("ButtonExit").GetComponent<Button>();
        btnExit.onClick.AddListener(OnBtnExitClicked);
    }

    void OnBtnExitClicked()
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(CityState));
    }
}
