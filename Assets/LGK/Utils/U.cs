using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class U
{
    public static T Closest<T>(this IEnumerable<T> stuff, GameObject to) where T : Component
    {
		return stuff.MinBy(to.Distance);
    }


	public static float Max(this Vector3 m) => Mathf.Max(m.x, m.y, m.z);
    public static float Pow2(this float x) => x * x;

    public static float Distance(this GameObject self, GameObject other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }


	public static bool Not(this GameObject obj) => !obj;
	public static bool Is(this GameObject obj) => obj;
	public static bool Is(this object obj) => obj != null;

    public static bool Is(this Tile obj) => obj;

    public static bool Not(this bool obj) => !obj;
	public static bool Not(this Component obj) => !obj;
	public static bool Is(this Component obj) => obj;

	public static float Distance(this Component self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }

    public static float Distance(this Component self, Vector3 other)
    {
        return Vector3.Distance(self.transform.position, other);
    }

	public static TValue Key<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => keyValuePair.Value;

    public static float Distance(this GameObject self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }

    public static Vector2 xz(this Vector3 self)
    {
        return new Vector2(self.x, self.z);
    }

	public static Vector3Int x0y(this Vector2Int self)
	{
		return new Vector3Int(self.x, 0, self.y);
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


    public static T WeightedRandom<T>(this Dictionary<T,float> self)
    {
        var total = self.Sum(x => x.Value);
        float target = UnityEngine.Random.value * total;

        //Debug.Log($"job rng is {target}/{total}");
        foreach (var thing in self)
        {
            if ((target -= thing.Value) < 0)
                return thing.Key;
        }

        return default(T);
    }

    public static Vector3 Mul(this Vector3 s, Vector3 o)
    {
        return new Vector3(s.x * o.x, s.y * o.y, s.z * o.z);
    }

	public static void Foreach<T>(this IEnumerable<T> collection, Action<T> f)
	{
		foreach (var item in collection)
		{
			f(item);
		}
	}

	public static T[] Find<T>(Vector3 pos, float range) where T:UnityEngine.Component
    {
        return GameObject.FindObjectsOfType<T>().Where(x => x.Distance(pos) <= range).ToArray();

    }


    public static T[] Find<T>(this GameObject self, float range) where T : UnityEngine.Component
    {
        return GameObject.FindObjectsOfType<T>().Where(x => self.Distance(x) <= range).ToArray();

    }
    public static IEnumerable<T> InRange<T>(this IEnumerable<T> self, Vector3 pos, float range) where T : UnityEngine.Component
    {
        return self.Where(x => x.Distance(pos) <= range);

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
