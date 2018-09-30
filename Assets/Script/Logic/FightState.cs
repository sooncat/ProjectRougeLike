using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightState : BaseGameState {

    public override void Init()
    {
        base.Init();
    }

    public override void Enter()
    {
        base.Enter();
        EventSys.Instance.AddEvent(LogicEvent.EnterFightState);
    }

    public override void Leave()
    {
        base.Leave();
        EventSys.Instance.AddEvent(LogicEvent.LeaveFightState);
    }

}
