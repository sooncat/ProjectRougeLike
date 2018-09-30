using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISystem : MonoBehaviour
{
    public virtual void Init() { }
    public void BaseReset() { Reset(); }
    public virtual void Reset() { }
    public virtual void Release() { }
    public virtual void SysUpdate() { }
    public virtual void SysLateUpdate() { }

    public string GetName()
    {
        return GetType().Name;
    }
}
