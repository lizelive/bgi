using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class U
{
    public static T Closest<T>(this IEnumerable<T> stuff) where T : Component
    {
        return default(T);
    }

    public static float Pow2(this float x) => x * x;

    public static float Distance(this GameObject self, GameObject other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }

    public static float Distance(this Component self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }

    public static float Distance(this Component self, Vector3 other)
    {
        return Vector3.Distance(self.transform.position, other);
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

    public static Vector3 x0z(this Vector3 self)
    {
        return new Vector3(self.x, 0, self.z);
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
        if (!self.Any())
            return default(T);
        return self.ToArray().Random();
    }


    public static T[] Find<T>(Vector3 pos, float range) where T:UnityEngine.Component
    {
        return GameObject.FindObjectsOfType<T>().Where(x => x.Distance(pos) <= range).ToArray();

    }

    public static T Closest<T>(this IEnumerable<T> self, Vector3 to) where T : UnityEngine.Component
    {
        return self.MinBy(x => x.Distance(to));
    }

    public static T MinBy<T>(this IEnumerable<T> self, Func<T, float> eval)
    {

        return self.MaxBy(x => -eval(x));
    }
}
