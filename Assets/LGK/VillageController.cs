using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static UnityEngine.Mathf;

public class VillageController : MonoBehaviour
{
	public Team team;
	public Player Player;
	public static VillageController I;

	public Offering offering;





	//public float Fear = 0;
	public float FearDecay = 0.1f;
	//public const float MaxFear = 10;

	public float FearPerVillagerKiled = 1;
	public int CurrentlyAliveVillager => Villagers.Count();


	public VillageController()
	{
		I = this;
	}


	public Villager villagerPrefab;

	public AnimationCurve fearCurve;

	public IEnumerable<VillagerHome> Houses => team.GetMembers<VillagerHome>();
	public IEnumerable<Villager> Villagers => team.GetMembers<Villager>();

	public void KilledAVillager(Health murder)
	{
		//print($"Murder. {murder} {murder?.team}");
		//if (murder?.team) team.AddRep(murder.team, -FearPerVillagerKiled);
		//Fear += FearPerVillagerKiled;
	}


	public void IBuilt(Team by, Building thing)
	{
		thing.Team = team;
		team.AddRep(by, thing.buildRep);
	}

	public double Balance
	{
		get
		{
			return team.Balance;
		}

		set
		{
			team.Balance = value;
		}
	}

	public float HumanReplaceCost = 200;
	public float HouseRepairCost = 100;

	public void DepositFood(float ammount)
	{

		//var fearp = 1 - team.GetRep(team) / team.TotalRep;

		var fear = 1 - team.Confidance;// fearCurve.Evaluate(fearp);
		if (float.IsNaN(fear))
		{
			fear = 0;
		}
		var payoutP = fearCurve.Evaluate(fear);
		//print($"{fear} is {payoutP}");


		offering.curret += payoutP * ammount;
		Balance += (1 - payoutP) * ammount;

	}
	public Mob heroPrefab;

	public Mob myHero;
	public float heroCost = 40;
	public float heroRep = -10;

	public float villageRadius = 32;
	public Transform spawnPoint;
	public CooldownTimer heroCooldown = new CooldownTimer(60);
	// Update is called once per frame
	void Update()
	{
		team.SetRep(team, Villagers.Count());
		var timeCorrectedDecay = FearDecay * CurrentlyAliveVillager;
		timeCorrectedDecay *= Time.deltaTime;


		var faction = team.reputations.Keys.FirstOrDefault(x => x != team);


		Team.Behavior behaviour = Team.Behavior.Normal;
		if(faction)
		{
			behaviour = team.WhatBehavior(faction);


			if (behaviour == Team.Behavior.Summon)
			{
				if (!myHero)
				{
					myHero = null;
					if (heroPrefab && Balance >= heroCost && heroCooldown.Check)
					{
						Balance -= heroCost;
						var spawnPos = spawnPoint.pos();
						myHero = Instantiate(heroPrefab, spawnPos, Quaternion.identity);
						myHero.transform.position = spawnPos;
						myHero.Team = team;
					}
				}

			}

			if(behaviour == Team.Behavior.Summon || behaviour == Team.Behavior.Fight)
			{
				var jobs = faction.mobs
					.Where(mob => gameObject.Distance(mob) <= villageRadius)
					.Select(mob => new MurderJob(mob.Health)).ToArray();



				foreach (var job in jobs)
				{
					team.JobManager.PublishJob(job);
				}
				//try
				//{
				//	jobs.Foreach();
				//}
				//catch(ArgumentException how)
				//{
				//	how.ToString();
				//}
				// probly make when they enter / leave?
			}
		}

		//print($"{team.name} in {behaviour} mode to {faction}");





		if (behaviour == Team.Behavior.Normal && Balance > HouseRepairCost)
		{
			var houseToFix = Houses.FirstOrDefault(x => x.state == VillagerHome.Status.Burnt);
			if (houseToFix)
			{
				Balance -= HouseRepairCost;
				houseToFix.state = VillagerHome.Status.Fine;
			}
		}

	}
}
