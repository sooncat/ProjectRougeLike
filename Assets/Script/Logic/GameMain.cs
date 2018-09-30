#define CATDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : BaseMain {

    protected override void GameStart()
    {
        
        CatDebug.Enable = true;
        CatDebug.Enabletype = CatLogType.CatEvent | CatLogType.CatTimer;
        CatDebug.RegistLogTypeLabel(CatLogType.CatEvent, "Event");
        CatDebug.RegistLogTypeLabel(CatLogType.CatTimer, "Timer");

        GameStateSys.Instance.InitState<LoadingState>();
        GameStateSys.Instance.InitState<LoginState>();
        GameStateSys.Instance.InitState<CityState>();
        GameStateSys.Instance.InitState<FightState>();
        GameStateSys.Instance.InitState<StartState>();

        InitSys<UISys>();

        ConfigSys.Instance.InitJsonConfig<GameStateConfig>(Application.streamingAssetsPath + "/GameConfig/GameStateConfig.json");

        //EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(StartState));
        
    }

    protected override void GameUpdate()
    {
        
    }

    protected override void GameLateUpdate()
    {

    }
}
