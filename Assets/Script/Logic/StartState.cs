using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : BaseGameState {

    bool _isNeedUpdate = false;

    protected override void OnAllPreLoaded()
    {
        base.OnAllPreLoaded();
        if(_isNeedUpdate)
        {
            
        }
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(LoginState));
    }

    
}
