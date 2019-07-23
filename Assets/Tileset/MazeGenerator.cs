using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator
{
	public int Size = 10;


	struct OpenData
	{
		public Vector2Int from;
		public float cost;
	}
	public IEnumerable<Tuple<Vector2Int, Vector2Int>> Generate()
	{
		var rng = new System.Random();

		var d = new int[Size, Size];

		var weights = new float[Size, Size];

		{
			var noise = new byte[(Size) * (Size)];

			rng.NextBytes(noise);


			int i = 0;
			for (int k = 0; k < Size; k++)
			{
				for (int j = 0; j < Size; j++)
				{

					weights[k, j] = noise[i] / 255f;

					i++;
				}
			}
		}


		var trom =
			new Vector2Int(
			rng.Next(Size),
			rng.Next(Size)
			);

		var closed = new HashSet<Vector2Int>();
		var open = new Dictionary<Vector2Int, OpenData>();
		open.Add(trom, new OpenData { from = trom, cost = 0 });
		while (!TileGrid.I.abort && open.Any())
		{
			var curent = open.MinBy(x => x.Value.cost);
			open.Remove(curent.Key);
			var pos = curent.Key;
			//Debug.Log($"{curent.Value.from}->{pos}:{curent.Value.cost}");

			yield return Tuple.Create(curent.Value.from, pos);

			closed.Add(pos);

			var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };


			foreach (var dir in dirs)
			{
				var newPos = pos + dir;
				if (!d.InBounds(newPos) || closed.Contains(newPos)) continue;
				var weight = weights.Index(newPos);
				var od = new OpenData
				{
					cost = weight,
					from = pos
				};

				if (open.TryGetValue(newPos, out OpenData oldWeight))
					od = new[] { od, oldWeight }.MinBy(x => x.cost);

				open[newPos] = od;
			}
		}
	}


}