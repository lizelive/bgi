using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class U
{
    public static float Distance(this GameObject self, GameObject other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }
    public static float Distance(this Component self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }
    public static float Distance(this GameObject self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }

    public static Vector2 xz(this Vector3 self)
    {
        return new Vector2(self.x, self.z);
    }


    public static Vector3 x0y(this Vector2 self)
    {
        return new Vector3(self.x, 0, self.y);
    }

    public static Vector3 pos(this Component self)
    {
        return self.transform.position;
    }

    public static T MaxBy<T>(this IEnumerable<T> self, Func<T, float> eval)
    {
        var bestVal = float.NegativeInfinity;
        var best = self.FirstOrDefault();
        foreach (var item in self)
        {
            var val = eval(item);
            if(val > bestVal)
            {
                bestVal = val;
                best = item;
            }
        }
        return best;
    }

    public static T Random<T>(this IList<T> self)
    {
        return self[UnityEngine.Random.Range(0, self.Count)];
    }

    public static T Random<T>(this IEnumerable<T> self)
    {
        return self.ToArray().Random();
    }


    public static T MinBy<T>(this IEnumerable<T> self, Func<T, float> eval)
    {
        return self.MaxBy(x => -eval(x));
    }
}
