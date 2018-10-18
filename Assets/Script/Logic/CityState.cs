using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityState : BaseGameState
{
    public override void Enter(GameStateParameter parameter)
    {
        base.Enter(parameter);

        EventSys.Instance.AddHander(InputEvent.CityExit, OnCityExitEvent);
        EventSys.Instance.AddHander(InputEvent.CityStartFight, OnCityStartFightEvent);
    }

    private void OnCityExitEvent(int eventId, object param1, object param2)
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(LoginState));
    }

    private void OnCityStartFightEvent(int eventId, object param1, object param2)
    {
        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(FightState), param1);
    }
}
