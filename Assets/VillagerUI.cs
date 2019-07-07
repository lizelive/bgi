using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VillagerUI : MonoBehaviour
{
	public Team village;

	public Text nameText, repText, popText, worthText, relationsText, fearText;

	public float maxFear = 1;

	public Sprite[] fearImages;

	public Image wheelImage, statusImage;
	public Transform wheelParent;
	Dictionary<Team, Image> teamWheels;


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


		if (fearText)
		{
			fearText.text = $"Fear {village.Fear:f2}";
		}
		if (statusImage)
		{
			var imageNumber = Mathf.FloorToInt(fearImages.Length * Mathf.Clamp01(village.Fear / maxFear));
			imageNumber = System.Math.Min(imageNumber, fearImages.Length - 1);
			statusImage.sprite = fearImages[imageNumber];

		}

		if (relationsText)
			relationsText.text = $"Rep {village.Confidance}\n" + string.Join("\n", village.reputations.Select(x => $"{x.Key.name} : {x.Value} : {village.WhatBehavior(x.Key)}"));
	}
}
