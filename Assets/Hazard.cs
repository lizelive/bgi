using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
   public DamageKind kind;
    public float rate;


    public void OnTriggerStay(Collider other)
    {
        var health = other.gameObject.GetComponent<Health>();
        if (health)
        {
            health.Hurt(Time.deltaTime * rate, DamageKind.Water);
        }
    }

}
