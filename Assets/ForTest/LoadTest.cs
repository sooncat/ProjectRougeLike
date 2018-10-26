using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LoadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        byte[] byteData = IOUtils.ReadFileFromStreamingAssets("hello.txt");
        string text = IOUtils.Byte2String(byteData);

        GetComponent<UINode>().GetRef("Text").GetComponent<Text>().text = text;

        string testStr = "\n\rtemp4567";
        IOUtils.SaveFile(testStr, "MyTest.kk");

        StartCoroutine(WaitForRead());

        
	}
	
	// Update is called once per frame
    IEnumerator WaitForRead()
    {
        yield return new WaitForSeconds(1);

        byte[] data2 = IOUtils.ReadFileFromPersistent("MyTest.kk");
        string text2 = IOUtils.Byte2String(data2);

        GetComponent<UINode>().GetRef("Text").GetComponent<Text>().text += text2;
	}
}
