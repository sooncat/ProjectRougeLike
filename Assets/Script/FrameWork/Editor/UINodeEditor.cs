using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UINode))]
public class UINodeEditor : Editor {

    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Init"))
        {
            UINode uiNode = (UINode)target;
            InitUiNode(uiNode);
        }
        GUILayout.EndHorizontal();
    }
	
    static void InitUiNode(UINode node)
    {
        node.AllRefs = new List<UIRef>();
        node.AllNodes = new List<UINode>();

        UINode[] uiNodes = node.GetComponentsInChildren<UINode>();
        foreach (UINode uiNode in uiNodes)
        {
            uiNode.AllRefs = new List<UIRef>();
            uiNode.AllNodes = new List<UINode>();
        }
        foreach (UINode uiNode in uiNodes)
        {   
            UINode parentNode = GetParentNode(uiNode.transform);
            if (parentNode)
            {
                parentNode.AllNodes.Add(uiNode);
            }
        }

        UIRef[] uiRefs = node.GetComponentsInChildren<UIRef>();
        foreach (UIRef uiRef in uiRefs)
        {
            UINode parentNode = GetParentNode(uiRef.transform);
            if(parentNode)
            {
                parentNode.AllRefs.Add(uiRef);
            }
        }
    }

    static UINode GetParentNode(Transform uiTrans)
    {
        Transform t = uiTrans.parent;
        while(t)
        {
            UINode uiNode = t.GetComponent<UINode>();
            if(uiNode)
            {
                return uiNode;
            }
            t = t.parent;
        };
        return null;
    }
}
