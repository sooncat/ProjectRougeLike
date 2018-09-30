using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginState : BaseGameState {

    
    public override void Init()
    {
        base.Init();
    }

    public override void Enter()
    {
        base.Enter();
        EventSys.Instance.AddEvent(LogicEvent.EnterLoginState);
    }

    public override void Leave()
    {
        base.Leave();
        EventSys.Instance.AddEvent(LogicEvent.LeaveLoginState);
    }
}
