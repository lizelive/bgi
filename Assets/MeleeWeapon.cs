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
    private Health health;

    public bool pierce;
    Animator animator;
    public void Start()
    {
        health = GetComponentInParent<Health>();
        animator= GetComponentInParent<Animator>() ?? GetComponentInChildren<Animator>();
    }
    public void Attack()
    {
       
        var didAttack = false;
        if (Time.time - lastAttack > cooldown)
        {
            animator.SetTrigger("Attack");
            lastAttack = Time.time;
            var stuffToHurt = FindObjectsOfType<Health>().Where(x=>this.Distance(x)<attackRange).Select(x => x.GetComponent<Health>()).Where(x => x);
            foreach (var thing in stuffToHurt)
            {
                if (thing.Hurt(damage, DamageKind.Blunt, health?.team, health))
                {
                    Debug.DrawLine(transform.position, thing.transform.position, Color.red, cooldown);
                    didAttack = true;
                    if (!pierce)
                        break;
                }
            }
        }
        
    }
}
