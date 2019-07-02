using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Dropdown;

public class BuildMenu : MonoBehaviour
{
	public Team team;
	public Building[] buildings;
	public Building[] ValidBuildings => buildings;
	public Building Current => ValidBuildings[dropdown.value];


	public UnityEngine.UI.Dropdown dropdown;

	private void Start()
	{
		dropdown.options = buildings.Select(b => new OptionData($"{b.name}: {b.cost}", b.looksLike)).ToList();
	}
	

	public int index;
	public void Next()
	{
		index++;
	}
}
