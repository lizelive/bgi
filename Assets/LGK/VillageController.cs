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

		if(murder?.team)
		team.AddRep(murder.team, -FearPerVillagerKiled);
        //print("Murder.");
        //Fear += FearPerVillagerKiled;
    }
    public float Balance
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
		
        var fearp = 1 - team.GetRep(team) / team.TotalRep;

		var payoutP = 0;// fearCurve.Evaluate(fearp);

        offering.curret += payoutP * ammount;
        Balance += (1 - payoutP) * ammount;

    }
	public Mob heroPrefab;
	public float heroFearTreshold = 18;
	public float heroFearRestored = 5;

	public Transform spawnPoint;

	public Mob myHero;

	public float Fear;

	// Update is called once per frame
	void Update()
    {
		team.mobs = new HashSet<Mob>(Villagers.Select(x => x.GetComponent<Mob>()));
		team.SetRep(team, Villagers.Count());
		var timeCorrectedDecay = FearDecay * CurrentlyAliveVillager;
        timeCorrectedDecay *= Time.deltaTime;


		Fear = 1 - team.Confidance;

		if (!myHero)
		{
			myHero = null;
			if (Fear > heroFearTreshold && heroPrefab)
			{
				var spawnPos = spawnPoint.pos();

				Fear -= heroFearTreshold;
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
