using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


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
	public int CurrentlyAliveVillager => Villagers.Length;


	public VillageController()
	{
		I = this;
	}


	public Villager villagerPrefab;

	public AnimationCurve fearCurve;

	public VillagerHome[] Houses => FindObjectsOfType<VillagerHome>();
	public Villager[] Villagers => FindObjectsOfType<Villager>();

	public void KilledAVillager(Health murder)
	{
		print($"Murder. {murder} {murder?.team}");
		if (murder?.team)
			team.AddRep(murder.team, -FearPerVillagerKiled);
		//Fear += FearPerVillagerKiled;
	}


	public void IBuilt(Team by, Building thing)
	{
		thing.Team = team;
		team.AddRep(by, thing.buildRep);
	}

	public Double Balance
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

		var fear = 1-team.Confidance;// fearCurve.Evaluate(fearp);
		if (float.IsNaN(fear))
		{
			fear = 0;
		}
		var payoutP = fearCurve.Evaluate(fear);
		print($"{fear} is {payoutP}");


		offering.curret += payoutP * ammount;
		Balance += (1 - payoutP) * ammount;

	}
	public Mob heroPrefab;

	public Mob myHero;
	public float heroCost = 40;
	public float heroRep = -10;


	public Transform spawnPoint;

	// Update is called once per frame
	void Update()
	{
		team.mobs = new HashSet<Mob>(Villagers.Select(x => x.GetComponent<Mob>()));
		team.SetRep(team, Villagers.Count());
		var timeCorrectedDecay = FearDecay * CurrentlyAliveVillager;
		timeCorrectedDecay *= Time.deltaTime;


		//Fear = 1 - team.Confidance;

		if (!myHero)
		{
			myHero = null;
			var worstTeam = team.reputations.MinBy(x => x.Value);
			if (worstTeam.Value < heroRep && heroPrefab && Balance >= heroCost)
			{
				Balance -= heroCost;
				var spawnPos = spawnPoint.pos();
				myHero = Instantiate(heroPrefab, spawnPos, Quaternion.identity);
				myHero.transform.position = spawnPos;
				myHero.Team = team;
			}
		}


		if (Balance > HouseRepairCost)
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
