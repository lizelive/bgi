using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float cooldown = 1;
    public float damage = 1;
    private float lastAttack;
    public float attackRange = 1.2f;


    public void Attack()
    {
        if (Time.time - lastAttack > cooldown)
        {
            var stuffToHurt = Physics.OverlapSphere(transform.position, attackRange).Select(x => x.GetComponent<Health>()).Where(x => x);
            foreach (var thing in stuffToHurt)
            {
                thing.Hurt(damage, DamageKind.Blunt, GetComponent<Health>()?.team);
            }
        }
    }
}
