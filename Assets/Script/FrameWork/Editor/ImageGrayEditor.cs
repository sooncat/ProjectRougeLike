using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ImageGray))]
public class ImageGrayEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        ImageGray ig = (ImageGray)target;
        ig.Gray = GUILayout.Toggle(ig.Gray, "Gray");
    }
}
