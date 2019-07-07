using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodAlter : MonoBehaviour
{
    public Team team;
    public float cost;
    public Mob spawns;

    public ParticleSystem spawnParticles;
    public ParticleSystem despawnParticles;

    public int freeRealestate;

	public void Update()
	{
		// play creepy sounds
	}

	public void OnCollisionEnter(Collision collision)
	{
		var mob = collision.transform.GetComponentInParent<Mob>();
		var player = collision.transform.GetComponentInParent<Player>();
		//print($"oh something has hit me {collision} {mob}");
		if (mob && !player)
		{
			Despawn(mob);
		}
	}

	public void Spawn(Player by)
    {
		if (team.mobs.Count > team.maxNumMobs)
			return;

		if (by.balance >= spawns.cost)
		{
            by.balance -= spawns.cost;

            //print($"bought and have {by.balance}");
            DoSpawn(by);
        }
    }

    void DoSpawn(Player by)
    {
        var noob = Instantiate(spawns, by.pos()+ by.transform.forward * 1, Quaternion.identity);
        noob.Health.team = team;
		noob.name = $"{English.RandomName()} {spawns.name} of {team.name}";

        var norb = noob.GetComponent<Norb>();
        if (norb) {
            norb.owner = by;
			
        }
    }

    public void Despawn(Mob mob)
    {
		team.balance += mob.cost;
		Destroy(mob.gameObject);
    }
   
}
