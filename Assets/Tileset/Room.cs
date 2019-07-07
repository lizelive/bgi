using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
	//map
	public TileGrid grid;
	// bounds
	public Vector2Int pos, neg;
    // Start is called before the first frame update
    public static bool IsValid(TileGrid grid, Region2D region)
	{
		return region.Select(U.x0y).Select(grid.Get).All(U.Is);
	}
}




public struct Region3D
{
	public void MinMax(ref int minout, ref int maxout)
	{
		if (minout > maxout)
		{
			var t = minout;
			minout = maxout;
			maxout = t;
		}
	}
	public Vector3Int max, min;
}
public struct Region2D : IEnumerable<Vector2Int>{

	/// <summary>
	/// the bounds. inclusive
	/// </summary>
	public Vector2Int max, min;

	public void MinMax(ref int minout, ref int maxout)
	{
		if(minout > maxout)
		{
			var t = minout;
			minout = maxout;
			maxout = t;
		}
	}

	public Region2D(Vector2Int a, Vector2Int b)
	{
		max = a.Zip(b, Math.Max);
		min = a.Zip(b, Math.Min);
	}

	public override string ToString()
	{
		return $"[{min}-{max}]";
	}

	public IEnumerator<Vector2Int> GetEnumerator()
	{
		var i = new Vector2Int();
		for (i.x = min.x; i.x <= max.x; i.x++)
		{
			for (i.y = min.y; i.y <= max.y; i.y++)
			{
				yield return i;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}