using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
	[SerializeField]
	private Mesh mesh;
	public Mesh Mesh => mesh;

    // Update is called once per frame
    void Tick()
    {
        // i promise we aren't playing minecraft
    }

	bool BlockUpdate()
	{
		return false;
	}
}
