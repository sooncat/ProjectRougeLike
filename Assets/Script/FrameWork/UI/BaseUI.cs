using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour {

    public int Id
    {
        get;
        private set;
    }

    public string Name
    {
        get;
        private set;
    }

    public virtual void InitUI(UINode rootNode)
    {
        
    }

    public void Init(int id)
    {
        Id = id;
        //Name = uiEnum.ToString();
    }

    public bool Equals(BaseUI cwindow)
    {
        return Id == cwindow.Id;
    }

    public override int GetHashCode()
    {
        return (Id) * 1000 + Name.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        BaseUI idObj = obj as BaseUI;
        if (idObj == null)
        {
            return false;
        }
        return Equals(idObj);
    }
    
}
