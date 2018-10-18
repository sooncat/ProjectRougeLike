using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : BaseView
{
    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        Button btnLogin = rootNode.GetRef("ButtonLogin").GetComponent<Button>();
        btnLogin.onClick.AddListener(OnBtnLoginClicked);
    }

    void OnBtnLoginClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.LoginLogin, typeof(CityState));
    }
}
