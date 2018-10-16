#define CATDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : BaseMain {

    protected override void GameStart()
    {
        
        CatDebug.Enable = true;
        CatDebug.Enabletype = CatLogType.CatTimer | CatLogType.CatEvent;
        CatDebug.RegistLogTypeLabel(CatLogType.CatEvent, "Event");
        CatDebug.RegistLogTypeLabel(CatLogType.CatTimer, "Timer");

        InitSys<UISys>();
        ConfigSys.Instance.InitJsonConfig<GameStateConfig>(Application.streamingAssetsPath + "/GameConfig/GameStateConfig.json");

        ConfigDataMgr.Instance.Load<HeroTableData>();
        ConfigDataMgr.Instance.Load<MonsterTableData>();
        ConfigDataMgr.Instance.Load<ItemTableData>();

        PlayerDataMgr.Instance.Init();
        FightDataMgr.Instance.Init();

        GameStateSys.Instance.InitState<LoginState>();
        GameStateSys.Instance.InitState<CityState>();
        GameStateSys.Instance.InitState<FightState>();
        GameStateSys.Instance.InitState<StartState>();

        EventSys.Instance.AddEvent(LogicEvent.ChangeState, typeof(StartState));
    }

    protected override void GameUpdate()
    {
        
    }

    protected override void GameLateUpdate()
    {

    }
}
