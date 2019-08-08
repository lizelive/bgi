﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public Team team;
   public DamageKind kind;
    public float rate;

    
    public void OnTriggerStay(Collider other)
    {
        var health = other.gameObject.GetComponent<Health>();
        if (health)
        {
            health.Hurt(Time.fixedDeltaTime * rate, kind, byTeam: team, ignoreCooldown:true);
        }
    }

}
