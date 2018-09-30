using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINode : MonoBehaviour {

    /// <summary>
    /// 可以自定义的名字，缺省则按照obj的名字来命名
    /// </summary>
    public string Label;
    public List<UIRef> AllRefs;
    public List<UINode> AllNodes;

    public Transform GetRef(string refLabel)
    {
        foreach (UIRef uiRef in AllRefs)
        {
            if (uiRef.Label.Equals(refLabel) || uiRef.gameObject.name.Equals(refLabel))
            {
                return uiRef.transform;
            }
        }
        return null;
    }

    public UINode GetNode(string nodeLabel)
    {
        foreach (UINode uiNode in AllNodes)
        {
            if (uiNode.Label.Equals(nodeLabel) || uiNode.gameObject.name.Equals(nodeLabel))
            {
                return uiNode;
            }
        }
        return null;
    }
}
