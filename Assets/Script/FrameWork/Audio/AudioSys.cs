using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSys : ISystem
{
    public static AudioSys Instance = null;
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

    public void UnloadSounds()
    {
        
    }
}
