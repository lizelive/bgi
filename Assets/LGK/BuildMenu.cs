using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
	public Team team;
	public Building[] buildings;
	public Building[] ValidBuildings => buildings;
	public Building Current => ValidBuildings[index % ValidBuildings.Length];
	
	public int index;
	public void Next()
	{
		index++;
	}
}
