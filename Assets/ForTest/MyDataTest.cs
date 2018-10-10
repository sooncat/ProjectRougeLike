using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using com.initialworld.framework;

public class MyDataTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
        
        StringBuilder sb = new StringBuilder();

        ENum<int> myNumber1 = new ENum<int>(1);
        for (int i = 0; i < 100;i++ )
        {
            myNumber1.Value += 10;
            sb.Append(myNumber1.Value).Append(",");
        }
        Debug.Log("myNumber1 = " + sb);
        sb = new StringBuilder();
        ENum<float> myNumber2 = new ENum<float>(1.1f);
        for (int i = 0; i < 100; i++)
        {
            myNumber2.Value += 1.1f;
            sb.Append(myNumber2.Value).Append(",");
        }
        Debug.Log("myNumber2 = " + sb);
        sb = new StringBuilder();
        ENum<double> myNumber3 = new ENum<double>(1.2d);
        for (int i = 0; i < 100; i++)
        {
            myNumber3.Value += 1.2f;
            sb.Append(myNumber3.Value).Append(",");
        }
        Debug.Log("myNumber3 = " + sb);
	}
	
}
