using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceSys : ISystem
{

    public static ResourceSys Instance = null;
    private bool _isInited = false;
    public override void Init()
    {
        if (_isInited)
        {
            return;
        }
        _isInited = true;
        Instance = this;
    }

    public void Clear()
    {
        
    }


    public GameObject CreateUI(string uiPrefabPath)
    {
        //todo: load and ins obj.
        return null;
    }
    
    public Sprite GetSprite(string iconPath)
    {
        //if(!iconPath.EndsWith(".png"))
        //{
        //    iconPath += ".png";
        //}
        Sprite result = Resources.Load<Sprite>(iconPath);
        if(result == null)
        {
            throw new FileNotFoundException(iconPath);
        }
        return result;
    }

    public Sprite GetFrame(int lv)
    {
        string framePath = GameConstants.FramePath + lv;
        return GetSprite(framePath);
    }
    
}
