using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LoadTest1 : MonoBehaviour {

	// Use this for initialization
	void Start () {

        string s = Resources.Load<TextAsset>("Configs/GameStateConfig").text;
        GetComponent<Text>().text = s;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
