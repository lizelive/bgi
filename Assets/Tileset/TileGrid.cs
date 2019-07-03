using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
	public static TileGrid I;
	TileGrid()
	{
		I = this;
	}

	public Tile Get(Vector2Int pos) => storage[pos];

	public Tile this[Vector2Int index]
	{
		get => storage[index];

		set => storage[index] = value;
	}

	UnboundArray2D<Tile> storage = new UnboundArray2D<Tile>();

	public Tile prefab;

	public Transform targeter, buildPreview;

	public float gridSize = 4f;

	public Tuple<Vector2Int, Vector2Int>[] maze;
	// Start is called before the first frame update
	void Start()
	{
		var mazeSize = 10;
		var gridSize = 2 * mazeSize + 1;

		var build = new bool[gridSize, gridSize];

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				var even = x % 2 + y % 2 == 2;
				build[x, y] = !even;
			}
		}

		maze = new MazeGenerator().Generate().ToArray();
		foreach (var path in maze)
		{
			var gapPoint = path.Item1 + path.Item2;
			build.Index(gapPoint + Vector2Int.one, false);
		}


		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				var pos = new Vector3Int(x, 0, y);
				pos.y = build.Index(pos.xz()) ? 1 : 0;
				Build(pos);
			}
		}
	}
	/*
	 * so game dev question. how far designed is your 
	*/
	// Update is called once per frame


	public bool showPreview = false;


	Vector2Int lastSelect, cell2d;


	void Build(Vector3Int pos)
	{
		Vector2Int cell2d = pos.xz();
		var has = storage[cell2d];
		if (has)
		{
			print($"{cell2d} has {has}");
		}
		else
		{
			var yum = Instantiate(prefab, gridSize * (Vector3)pos, Quaternion.identity);
			yum.transform.localScale = Vector3.one * gridSize;
			storage[cell2d] = yum;
		}
	}

	void Break(Vector2Int cell)
	{
		var boi = this[cell];
		print($"Break {cell} {boi}");
		if (boi)
		{
			Destroy(this[cell].gameObject);
			this[cell] = null;
		}
	}

	void Update()
	{

		//foreach (var move in maze)
		//{
		//	Debug.DrawLine(move.Item1.x0y(), move.Item2.x0y());
		//}

		var doSelect = Input.GetKeyDown(KeyCode.K);
		var toggleView = Input.GetKeyDown(KeyCode.J);
		var deleteBlock = Input.GetKeyDown(KeyCode.L);

		showPreview ^= toggleView;

		buildPreview.gameObject.SetActive(showPreview);

		var pos = targeter.pos();
		pos += gridSize / 2 * targeter.up;
		var cell = (pos / gridSize).FloorToInt();


		pos = gridSize * (Vector3)cell;
		buildPreview.localScale = Vector3.one * gridSize;
		buildPreview.position = pos;
		cell2d = cell.xz();


		// slect logic


		if (doSelect)
		{
			var selected = new Region2D(lastSelect, cell2d);
			var validSelect = Room.IsValid(this, selected);
			print($"select was {validSelect}, {selected}");
			lastSelect = cell2d;
		}

		//delete logic
		if (deleteBlock)
		{
			Break(cell2d);
		}


		// build logic
		if (Input.GetKeyDown(KeyCode.U))
		{
			Build(cell);
		}

	}

	public bool abort;
}


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