using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public static class PhysicsUtils
{
    /// <summary>
    /// Calculated
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="launchVel"></param>
    /// <param name="throwHigh"></param>
    /// <returns></returns>
    public static Vector3 ComputeThrow(Vector3 start, Vector3 target, float launchVel, bool throwHigh = true)
    {
        var y = target.y - start.y;

        var startPos2d = start.xz();
        var targetPos2d = target.xz();

        var distance = Vector2.Distance(startPos2d, targetPos2d);

        var gravity = 9.81f;

        var v = launchVel;
        var v2 = v * v;
        var d2 = Mathf.Pow(distance, 2);

        var y2 = Mathf.Pow(y, 2);

        //var vy0 = -Mathf.Sqrt(-(2*d2*g*y) / (d2 + y2) + (d2*LV)/ (d2 + y2) + (2*LV*y2)/ (d2 + y2) - Mathf.Sqrt(-d ^ 4(4*d2*g2 - 4*g*LV*y - Mathf.Pow(LV, 2))) / (d ^ 2 + y ^ 2))/ sqrt(2)

        var det = Mathf.Sqrt(v2 * v2 - gravity * (gravity * d2 + 2 * y * v2));
        if (!throwHigh) det = -det;
        var theta = Mathf.Atan2(v2 + det, gravity * distance);
        var vy = Mathf.Sin(theta) * v;
        var vx = Mathf.Cos(theta) * v;

        return Vector3.up * vy + vx * (targetPos2d - startPos2d).normalized.x0y();
    }
}
