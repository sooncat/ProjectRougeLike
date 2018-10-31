using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtils {

    public static Vector3 GetEulerAngle(Vector3 p1, Vector3 p2)
    {
        //float centerX = (p1.x + p2.x) / 2;
        //float centerY = (p1.y + p2.y) / 2;

        float deltaY = p2.y - p1.y;
        float deltaX = p2.x - p1.x;
        double arc = System.Math.Atan(deltaY / deltaX);
        double angle = (180 / Mathf.PI) * arc;
        if (deltaX < 0)
        {
            angle += 180;
        }

        Vector3 result = new Vector3(0, 0, (float)angle);
        return result;
    }

    public static void ScrollToTop(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
    
    public static void DestroyChildren(this Transform trans)
    {
        List<Transform> result = new List<Transform>();
        foreach(Transform t in trans)
        {
            result.Add(t);
        }
        for (int i = 0; i < result.Count;i++ )
        {
            Object.Destroy(result[i].gameObject);
        }
    }
}
