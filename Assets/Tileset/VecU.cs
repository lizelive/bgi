using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VecU
{
    public static Vector2Int Div(this Vector2Int pos, int by)
	{
		return new Vector2Int(pos.x / by, pos.y / by);
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
}
