using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSys : ISystem {


    public static SceneSys Instance = null;
    private bool _isInited = false;
    public override void Init()
    {
        if (_isInited)
        {
            return;
        }
        _isInited = true;
        Instance = this;

        EventSys.Instance.AddHander(LogicEvent.SceneLoadStart, OnLoadSceneStart);
    }

    void OnLoadSceneStart(object p1, object p2)
    {
        string sceneName = (string)p1;
        bool isAdditive = (bool)p2;
        StartCoroutine(LoadSceneDsync(sceneName, isAdditive));
    }

    IEnumerator LoadSceneDsync(string sceneName, bool isAdditive)
    {
        //CatDebug.LogFunc(sceneName);
        CatTimer.StartRecord(sceneName, 0);

        Resources.UnloadUnusedAssets();
        yield return new WaitForEndOfFrame();

        AsyncOperation asyncLoad;

        if(isAdditive)
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);    
        }

        float nowProgress = 0;
        while (!asyncLoad.isDone)
        {
            if(asyncLoad.progress > nowProgress)
            {
                nowProgress = asyncLoad.progress;
                EventSys.Instance.AddEvent(LogicEvent.SceneLoadProgressChanged, nowProgress);
            }
            yield return new WaitForEndOfFrame();
        }

        EventSys.Instance.AddEvent(LogicEvent.SceneLoadEnd);
        //EventSys.Instance.AddEvent(EEventType.SDK_Bugly_Record, "Scene", m_loadSceneName);
        //CSceneRessLoadSys.Instance.UnloadSceneAB(false);

        CatTimer.EndRecord(sceneName);

        
    }


}
