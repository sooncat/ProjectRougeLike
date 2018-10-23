using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LoadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Sprite s = Resources.Load<Sprite>("Icon/cards/blankplus");
        GetComponent<Image>().sprite = s;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
