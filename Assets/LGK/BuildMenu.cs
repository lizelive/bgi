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
	public Building Current => dropdown.value == 0 ? null : ValidBuildings[dropdown.value - 1];


	public UnityEngine.UI.Dropdown dropdown;

	private void Start()
	{

		var stuff = new List<OptionData>(new[] { new OptionData($"Nothing", null) });
		stuff.AddRange(buildings.Select(b => new OptionData($"{b.name}: {b.cost}", b.looksLike)));
		dropdown.ClearOptions();
		dropdown.AddOptions(stuff);
		dropdown.value = 0;
	}
	

	public int index;
	public void Next()
	{
		index++;
	}
}
