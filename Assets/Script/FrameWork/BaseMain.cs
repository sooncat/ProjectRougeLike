using System.Collections;
using System.Collections.Generic;
using com.initialworld.framework;
using UnityEngine;
using UnityEngine.Analytics;

public class BaseMain : MonoBehaviour {

    protected List<ISystem> Sys;

	// Use this for initialization
	void Start () {

        Screen.SetResolution(640, 960, false);

        DontDestroyOnLoad(gameObject);

        Sys = new List<ISystem>();

        InitSys<EventSys>();
        InitSys<AssetBundleSys>();
        InitSys<ResourceSys>();
        InitSys<AudioSys>();
        InitSys<GameStateSys>();
        InitSys<SceneSys>();
        InitSys<ConfigSys>();
        InitSys<PreLoadSys>();

        gameObject.AddComponent<TimerUtils>();
        gameObject.AddComponent<CoroutineUtils>();

        GameStart();
	}
	
    protected void InitSys<T>() where T : ISystem
    {
        ISystem sys = gameObject.AddComponent<T>();
        sys.Init();
        Sys.Add(sys);
    }
    

	// Update is called once per frame
	void Update () {
	    foreach (ISystem iSys in Sys)
	    {
	        iSys.SysUpdate();
	    }
        GameUpdate();
	}

    void LateUpdate()
    {
        foreach (ISystem iSys in Sys)
        {
            iSys.SysLateUpdate();
        }
        GameLateUpdate();
    }

    protected virtual void GameStart()
    {

    }

    protected virtual void GameUpdate()
    {

    }

    protected virtual void GameLateUpdate()
    {

    }
}
