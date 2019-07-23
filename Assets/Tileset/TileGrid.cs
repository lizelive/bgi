using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
	public static TileGrid I;
	TileGrid()
	{
		I = this;
	}

	public Tile Get(Vector3Int pos) => storage[pos];

	public Tile this[Vector3Int index]
	{
		get => storage[index];

		set => storage[index] = value;
	}

	//[SerializeField]
	TileUnboundArray3D storage = new TileUnboundArray3D();

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


	Vector3Int lastSelect, cell2d;


	void Build(Vector3Int pos)
	{
		var has = this[pos];
		if (!has)
		{
			var yum = Instantiate(prefab, gridSize * (Vector3)pos, Quaternion.identity);
			yum.transform.localScale = Vector3.one * gridSize;
			this[pos] = yum;
		}
	}

	void Break(Vector3Int cell)
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

		var doSelect = InMan.Melee;
		var toggleView = InMan.BuildmodeMC;
		var deleteBlock = InMan.BreakMC;

		showPreview ^= toggleView;

		buildPreview.gameObject.SetActive(showPreview);

		var pos = targeter.position;
		var selectedCell = (pos / gridSize - 0.1f*targeter.up).RoundToInt();
		pos += gridSize/2 * targeter.up;
		var cell = (pos / gridSize).RoundToInt();
		

		pos = gridSize * (Vector3)cell;
		buildPreview.localScale = Vector3.one * gridSize;
		buildPreview.position = gridSize * (Vector3)selectedCell;
		cell2d = cell;


		// slect logic


		//if (doSelect)
		//{
		//	var selected = new Region2D(lastSelect, cell2d);
		//	var validSelect = Room.IsValid(this, selected);
		//	print($"select was {validSelect}, {selected}");
		//	lastSelect = cell2d;
		//}

		//delete logic
		if (deleteBlock)
		{
			Break(selectedCell);
		}


		// build logic
		if (InMan.BuildMC)
		{
			Build(cell);
		}

	}


	public bool abort;
}
