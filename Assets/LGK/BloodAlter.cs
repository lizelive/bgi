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

    public void Spawn(Player by)
    {
        if(freeRealestate > 0)
        {
            print("Free realestate");
            freeRealestate--;
            DoSpawn(by);
            return;
        }else if(by.balance >= cost)
        {
            by.balance -= cost;

            print($"bought and have {by.balance}");
            DoSpawn(by);
        }
    }

    void DoSpawn(Player by)
    {
        var noob = Instantiate(spawns, transform.position + Vector3.up * 3, Quaternion.identity);
        noob.Health.team = team;
        var norb = noob.GetComponent<Norb>();
        if (norb) {
            norb.owner = by;
        }
    }

    public void Despawn(Mob mob)
    {
        Destroy(mob);
        freeRealestate++;
    }
   
}
