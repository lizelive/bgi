using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VillagerUI : MonoBehaviour
{
	public Team village;

	public Text nameText, repText, popText, worthText;


	public Image wheelImage;
	public Transform wheelParent;
	Dictionary<Team, Image> teamWheels;
	// Start is called before the first frame update
	void Start()
	{


		var images = wheelParent.GetComponentInChildren<Image>();
	}


	// Update is called once per frame
	void Update()
	{
		nameText.text = village.name;
		popText.text = $"Population {village.mobs.Count}";
		worthText.text = $"${village.Balance:f2}";
		var c = village.color;
		c.a = 1;
		wheelImage.color = c;
		wheelImage.fillMethod = Image.FillMethod.Radial360;
		wheelImage.fillAmount = village.Confidance;

		print($"{village.reputation.Count}/n"+string.Join("/n", village.reputation.Select(x => $"{x.Key} : {x.Value}")));

		if (village.Confidance > 1)
			print($"I am so humble {village.Confidance}");
	}
}
