using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{

	UnboundArray2D<GameObject> storage = new UnboundArray2D<GameObject>();

	public GameObject prefab;

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
	void Update()
	{
		var toggleView = Input.GetKeyDown(KeyCode.J);

		
		if (toggleView)
			buildPreview.gameObject.SetActive(showPreview ^= toggleView);

		

		var pos = targeter.pos();
		pos += gridSize / 2 * targeter.up;
		var cell = (pos / gridSize).FloorToInt();


		pos = gridSize * (Vector3)cell;
		buildPreview.localScale = Vector3.one * gridSize;
		buildPreview.position = pos;

		if (Input.GetKeyDown(KeyCode.U))
		{

			var cell2d = cell.xz();

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
