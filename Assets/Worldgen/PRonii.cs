using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class PRonii
{

    public Vector2 Hash(Vector2 point)
    {
        //var hash = point.ToString().GetHashCode();
        //var o = new Vector2();
        //o.y = hash & 0xFFFF; 
        //o.x=hash >> 16;
        //o /= 0xFFFF;


        point *= 1000;

        return new Vector2(Mathf.PerlinNoise(point.x, point.y), Mathf.PerlinNoise(point.y, point.x));
    }

    public float gridSize = 1;

    public Vector2 GetNearest(Vector2 point) {

        var cell = Vector2Int.RoundToInt(point / gridSize);



        var minDistance = float.PositiveInfinity;
        var neaestPoint = Vector3.zero;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var tcell = cell + new Vector2Int(x, y);
                var cp = (tcell + Hash(tcell)) * gridSize;
                var dist = Vector2.Distance(cp, point);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    neaestPoint = cp;
                }
            }
        }

        return neaestPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
