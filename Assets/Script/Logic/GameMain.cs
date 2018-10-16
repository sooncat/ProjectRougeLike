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

        //test
        //HeroDataInfo hero1 = (HeroDataInfo)ConfigDataMgr.Instance.GetDataInfo<HeroTableData>(1);
        //Debug.Log("hero1 = " + hero1.Name);
        //MonsterDataInfo monster1 = (MonsterDataInfo)ConfigDataMgr.Instance.GetDataInfo<MonsterTableData>(1);
        //Debug.Log("monster1 = " + monster1.Name);
        //ItemDataInfo item1 = (ItemDataInfo)ConfigDataMgr.Instance.GetDataInfo<ItemTableData>(1);
        //Debug.Log("item1 = " + item1.Name);
        //test end

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
