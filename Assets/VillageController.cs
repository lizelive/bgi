using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class VillageController : MonoBehaviour
{
    public Player Player;
    public static VillageController I;

    public Offering offering;

    public float Fear = 0;
    public float FearDecay = 0.1f;
    public const float MaxFear = 10;

    public float FearPerVillagerKiled = 1;
    public int CurrentlyAliveVillager => Villagers.Length;

    public VillageController()
    {
        I = this;
    }

    public AnimationCurve fearCurve;

    public VillagerHome[] Houses => FindObjectsOfType<VillagerHome>();
    public Villager[] Villagers => FindObjectsOfType<Villager>();

    public void KilledAVillager()
    {
        print("Murder.");
        Fear += FearPerVillagerKiled;
    }
    public float Balance = 300;

    public float HumanReplaceCost = 200;
    public float HouseRepairCost = 100;

    public void DepositFood(float ammount)
    {
        var fearp = Fear / MaxFear;
        var payoutP =  fearCurve.Evaluate(fearp);

        offering.curret += payoutP * ammount;
        Balance += (1 - payoutP) * ammount;

    }

    // Update is called once per frame
    void Update()
    {
        Fear *= (1 - Mathf.Pow(FearDecay* CurrentlyAliveVillager, Time.deltaTime));

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
