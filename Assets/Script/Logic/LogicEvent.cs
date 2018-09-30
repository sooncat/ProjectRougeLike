using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public partial class LogicEvent {

    public const int StartSceneLoad = 5000;
    public const int SceneLoadProgressChanged = 5010;
    public const int EndSceneLoad = 5020;

    public const int UiPreLoadStart = 5050;
    public const int UiPreLoadProgressChanged = 5060;
    public const int UiPreLoadEnd = 5070;

    public const int AllPreLoadEnd = 6000;

    public const int ChangeState = 10000;
    public const int ChangeToNextState = 10001;

    public const int EnterLoginState = 10010;
    public const int LeaveLoginState = 10020;
    public const int EnterCityState = 10030;
    public const int LeaveCityState = 10040;
    public const int EnterFightState = 10050;
    public const int LeaveFightState = 10060;
    public const int EnterLoadingState = 10070;
    public const int LeaveLoadingState = 10080;

        
}
