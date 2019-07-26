using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Block
{

	public string name;
	public Mesh mesh;


	public static Block[] Types = 
	{
		new Block { name = "Air", mesh = new Mesh() },
		new Block { name = "Block", mesh = Default.I.buildingBlockMesh}
	};
		
		
}


[System.Serializable]
public struct Tile 
{
	public short blocktype;
	public object data;

	public Mesh Mesh => Block.Types[blocktype].mesh;

	public bool IsAir => blocktype == 0;

	public static Tile Air => new Tile();

	public static implicit operator bool(Tile d) => d.IsAir;


	bool BlockUpdate()
	{
		return false;
	}
}
