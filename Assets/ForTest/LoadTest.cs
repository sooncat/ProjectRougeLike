using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;

public class LoadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        byte[] byteData = IOUtils.ReadFileFromStreamingAssets("hello.txt");
        string text = IOUtils.Byte2String(byteData);

        GetComponent<UINode>().GetRef("Text").GetComponent<Text>().text = text;

        string testStr = "\n\rtemp4567";
        IOUtils.SaveFile(testStr, "MyTest.kk");

        StartCoroutine(WaitForRead());

        LoadABTest();

        StartCoroutine(LoadABAsync());

        LoadManifestTest();

        LoadTxtTest();
	}
	
	// Update is called once per frame
    IEnumerator WaitForRead()
    {
        yield return new WaitForSeconds(1);

        byte[] data2 = IOUtils.ReadFileFromPersistent("MyTest.kk");
        string text2 = IOUtils.Byte2String(data2);

        GetComponent<UINode>().GetRef("Text").GetComponent<Text>().text += text2;
	}

    void LoadABTest()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles/tex/icon/wings");
        if(!File.Exists(path))
        {
            Debug.LogError("Don't Find File in Path " + path);
        }
        Debug.Log("path = " + path);
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
        if (myLoadedAssetBundle == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        Sprite s = myLoadedAssetBundle.LoadAsset<Sprite>("ico_wing_cangyi");
        //Instantiate(prefab);

        GetComponent<UINode>().GetRef("Image").GetComponent<Image>().sprite = s;

        myLoadedAssetBundle.Unload(false);
    }

    IEnumerator LoadABAsync()
    {
        string uri = "file:///" + Application.streamingAssetsPath + "/AssetBundles/tex/icon/wings";
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri, 0);
        yield return request.Send();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        Sprite s = bundle.LoadAsset<Sprite>("ico_wing_fengchitianxiang");
        
        GetComponent<UINode>().GetRef("Image").GetComponent<Image>().sprite = s;
    }

    void LoadManifestTest()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles/AssetBundles");
        AssetBundle ab = AssetBundle.LoadFromFile(path);

        object[] all = ab.LoadAllAssets();
        foreach (object o in all)
        {
            Debug.Log("" +o.GetType());
        }

        AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        if (manifest == null)
        {
            Debug.LogError("no manifest");
        }
        string[] temp = manifest.GetAllDependencies("view/loadingui");
        foreach (string s in temp)
        {
            Debug.Log("dependencies = " + s);
        }
        temp = manifest.GetAllAssetBundles();
        foreach (string s in temp)
        {
            Debug.Log("abs = " + s);
        }
    }

    void LoadTxtTest()
    {
        string path = Application.streamingAssetsPath + "/AssetBundles/configs/gameconfig";
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        TextAsset ta = ab.LoadAsset<TextAsset>("GameStateConfig.json");
        Debug.Log(ta.text);

        //byte[] data = IOUtils.ReadFileFromStreamingAssets(path);
        //string str = IOUtils.Byte2String(data);
        //Debug.Log(str);

    }
}
