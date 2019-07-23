using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class VillagerHome : MonoBehaviour, IBurnable
{

	public Team Team => GetComponent<Health>().team;
	public Transform spawnPoint;
	public float costToSpawn;

    public enum Status
    {
        Fine,
        Burning,
        Burnt
    }

    public float fireStartTime;


    public Status state = Status.Fine;
    public GameObject fine;
    public GameObject burnt;
    public GameObject fire;
    

    public void Burn()
    {
        if (state != Status.Fine) return;
        fireStartTime = Time.time;
        state = Status.Burning;
    }

    public float fireBurnTime = 300;

	public float spawnCost = 10;

	public Mob villagerPrefab;
	public Mob villager;

	public Team Village => GetComponent<Health>().team;

	CooldownTimer spawnTimer = new CooldownTimer(60);
	// Update is called once per frame
	void Update()
    {

		if (!villager && spawnTimer.Check && Team.NumMobs >= 2 && Team.Balance >= costToSpawn)
		{
			Team.Balance -= costToSpawn;
			villager = null;
			villager = Instantiate(villagerPrefab, spawnPoint.pos(), Quaternion.identity);
			villager.Team = Village;
		}

        if (state==Status.Burning && Time.time-fireStartTime>fireBurnTime)
        {
            state = Status.Burnt;
        }

        switch (state)
        {
            case Status.Fine:
                fine.SetActive(true);
                burnt.SetActive(false);
                fire.SetActive(false);
                break;
            case Status.Burning:
                fine.SetActive(true);
                burnt.SetActive(false);
                fire.SetActive(true);
                break;
            case Status.Burnt:
                fine.SetActive(false);
                burnt.SetActive(true);
                fire.SetActive(false);
                break;
        }
    }
}