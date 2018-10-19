using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGray : MonoBehaviour {

    private bool _gray;
    public bool Gray
    {
        get { return _gray; }
        set
        {
            _gray = value;
            SetGray();
        }
    }

    void SetGray()
    {
        Shader s = Shader.Find(_gray ? "UI/Gray" : "UI/Default");
        GetComponent<Image>().material = new Material(s);
    }

}
