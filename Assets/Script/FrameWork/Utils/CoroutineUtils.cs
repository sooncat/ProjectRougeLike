using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtils : MonoBehaviour {


    private static CoroutineUtils _instance;
    public static CoroutineUtils Instance
    {
        get { return _instance; }
    }

    void Start()
    {
        _instance = this;
    }

    public void CreateCoroutine(IEnumerator ie)
    {
        StartCoroutine(ie);
    }

}
