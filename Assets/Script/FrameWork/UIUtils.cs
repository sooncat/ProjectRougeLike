using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtils {

    public static Vector3 GetEulerAngle(Vector3 p1, Vector3 p2)
    {
        float centerX = (p1.x + p2.x) / 2;
        float centerY = (p1.y + p2.y) / 2;

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
}
