using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLoadSys : ISystem {

    Stack<string> _resPath; 
    const int SimultaneousCount = 5;
    int _fullCount;
    int _loadedCount;

    public override void Init()
    {
        base.Init();

        _resPath = new Stack<string>();

        EventSys.Instance.AddHander(FrameEvent.AddPreLoadRes, OnAddPreLoadRes);
        EventSys.Instance.AddHander(FrameEvent.PreLoadStart, OnStartPreLoad);

        EventSys.Instance.AddHander(FrameEvent.EndLoadAssetBundleAsync, OnABLoaded);
    }

    void OnAddPreLoadRes(object p1, object p2)
    {
        string path = (string)p1;
        _resPath.Push(path);
    }

    void OnStartPreLoad(object p1, object p2)
    {
        _fullCount = _resPath.Count;
        int count = 0;
        while (++count < SimultaneousCount && _resPath.Count > 0)
        {
            string path = _resPath.Pop();
            EventSys.Instance.AddEvent(FrameEvent.StartLoadAssetBundleAsync, path);
        }
    }

    void OnABLoaded(object p1, object p2)
    {
        _loadedCount++;
        EventSys.Instance.AddEvent(FrameEvent.PreLoadUpdatePercent, _loadedCount, _fullCount);
        if(_resPath.Count > 0)
        {
            string path = _resPath.Pop();
            EventSys.Instance.AddEvent(FrameEvent.StartLoadAssetBundleAsync, path);
        }
        else
        {
            if(_loadedCount == _fullCount)
            {
                EventSys.Instance.AddEvent(FrameEvent.PreloadEnd);
            }
        }
    }


}
