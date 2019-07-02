using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{


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
	// Start is called before the first frame update
	void Start()
	{

	}
	/*
	 * so game dev question. how far designed is your 
	*/
	// Update is called once per frame


	public bool showPreview = false;


	Vector2Int lastSelect, cell2d;

	void Break(Vector2Int meow)
	{
		print($"Break {meow} {this[meow]}");
		Destroy(this[meow].gameObject);
		this[meow] = null;
	}

	void Update()
	{
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
			var has = storage[cell2d];
			if (has)
			{
				print($"{cell2d} has {has}");
			}
			else
			{
				var yum = Instantiate(prefab, pos, Quaternion.identity);
				yum.transform.localScale = Vector3.one * gridSize;
				storage[cell2d] = yum;
			}




		}

	}
}
