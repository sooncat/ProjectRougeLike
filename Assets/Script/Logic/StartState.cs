using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : BaseGameState {

    bool _isNeedUpdate = false;

    public override void Enter()
    {
        base.Enter();
        if(_isNeedUpdate)
        {
            EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(UpdateState));
        }
        else
        {
            EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(LoginState));
        }
    }
}
