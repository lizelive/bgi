using System;
using UnityEngine;

public static class VecU
{
    public static readonly Vector3Int[] CardnalDirs =
    {
        Vector3Int.left, Vector3Int.right,
        Vector3Int.FloorToInt(Vector3.forward),
        Vector3Int.FloorToInt(Vector3.back)
    };

    public static readonly Vector3Int[] PureDirs =
    {
        Vector3Int.up, Vector3Int.down,
        Vector3Int.left, Vector3Int.right,

        // fuck you unity
        Vector3Int.FloorToInt(Vector3.forward),
        Vector3Int.FloorToInt(Vector3.back)
    };

    public static Vector2Int Div(this Vector2Int pos, int by)
    {
        return new Vector2Int(pos.x / by, pos.y / by);
    }

    public static Vector3Int Div(this Vector3Int pos, int by)
    {


        return FloorToInt((1f / by) * (Vector3)pos);
    }

    public static Vector2Int xz(this Vector3Int self)
    {
        return new Vector2Int(self.x, self.z);
    }

    public static Vector3Int FloorToInt(this Vector3 pos)
    {
        return new Vector3Int(
        Mathf.FloorToInt(pos.x),
        Mathf.FloorToInt(pos.y),
        Mathf.FloorToInt(pos.z)
        );
    }


    public static Vector3Int RoundToInt(this Vector3 pos)
    {
        return new Vector3Int(
        Mathf.RoundToInt(pos.x),
        Mathf.RoundToInt(pos.y),
        Mathf.RoundToInt(pos.z)
        );
    }


    public static int ModPostive(this int value, int mod)
    {
        var x = value % mod;
        return x < 0 ? x + mod : x;

    }

    public static Vector2Int Zip(this Vector2Int a, Vector2Int b, Func<int, int, int> f)
    {
        return new Vector2Int(f(a.x, b.x), f(a.y, b.y));
    }
    public static T Index<T>(this T[,] array, Vector2Int pos)
    {
        return array[pos.x, pos.y];
    }

    public static bool InBounds<T>(this T[,] array, Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < array.GetLength(0) && pos.y < array.GetLength(1);
    }

    public static T Index<T>(this T[,] array, Vector2Int pos, T set)
    {
        return array[pos.x, pos.y] = set;
    }


    public static T Index<T>(this T[,,] array, Vector3Int pos)
    {
        return array[pos.x, pos.y, pos.z];
    }

    public static T Index<T>(this T[,,] array, Vector3Int pos, T set)
    {
        return array[pos.x, pos.y, pos.z] = set;
    }
}
