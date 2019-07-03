using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VillagerUI : MonoBehaviour
{
	public Team village;

	public Text nameText, repText, popText, worthText, relationsText;


	public Image wheelImage;
	public Transform wheelParent;
	Dictionary<Team, Image> teamWheels;
	// Start is called before the first frame update
	void Start()
	{



		//var images = wheelParent.GetComponentInChildren<Image>();
	}


	// Update is called once per frame
	void Update()
	{

		if (nameText)
			nameText.text = village.name;


		if (popText)
			popText.text = $"Population {village.mobs.Count} / {village.maxNumMobs}";


		if (worthText)
			worthText.text = $"${village.Balance:f2}";

		if (wheelImage)
		{
			wheelImage.color = village.color;
			wheelImage.fillAmount = village.Confidance;
		}
		if (relationsText)
			relationsText.text = $"Rep {village.Confidance}\n" + string.Join("\n", village.reputations.Select(x => $"{x.Key} :\t {x.Value}"));
	}
}
