using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DelayAction : MonoBehaviour {

    
    public float DelaySecond;
    public Action DAction;

    public void StartDelay()
    {
        gameObject.SetActive(true);
        StartCoroutine(DelayProgress());
    }

    IEnumerator DelayProgress()
    {
        yield return new WaitForSeconds(DelaySecond);
        if(DAction != null)
        {
            DAction();
        }
    }
}
