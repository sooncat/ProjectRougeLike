using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public partial class LogicEvent {

    public const int SceneLoadStart = 5000;
    public const int SceneLoadProgressChanged = 5010;
    public const int SceneLoadEnd = 5020;

    public const int UiLoadStart = 5050;
    public const int UiLoadProgressChanged = 5060;
    public const int UiLoadEnd = 5070;
    
    public const int AllPreLoadEnd = 6000;


    public const int UiLoadingStart = 7000;
    public const int UiLoadingUpdate = 7001;
    public const int UiLoadingEnd = 7002;
    

    public const int ChangeState = 10000;
    public const int LeaveState = 10010;
    public const int EnterState = 10020;

    public const int DrawFightUi = 20000;
        
}
