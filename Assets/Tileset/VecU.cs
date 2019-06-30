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
	public static T Index<T>(this T[,] array, Vector2Int pos)
	{
		return array[pos.x, pos.y];
	}

	public static T Index<T>(this T[,] array, Vector2Int pos, T set)
	{
		return array[pos.x, pos.y] = set;
	}
}
