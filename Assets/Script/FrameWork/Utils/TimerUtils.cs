using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerUtils : MonoBehaviour {

    private static TimerUtils _instance;
    public static TimerUtils Instance
    {
        get { return _instance; }
    }
	
    void Start()
    {
        _instance = this;
    }

    public void StartTimer(float second, Action action)
    {
        StartCoroutine(TimerProgress(second, action));
    }

    IEnumerator TimerProgress(float second, Action action)
    {
        yield return new WaitForSeconds(second);
        if(action != null)
        {
            action();
        }
    }

}
