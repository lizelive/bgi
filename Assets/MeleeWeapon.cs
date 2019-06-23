using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;
public class MeleeWeapon : MonoBehaviour
{
    public float cooldown = 1;
    public float damage = 1;
    private float lastAttack;
    public float attackRange = 1.2f;
    private Mob Mob;

    public float launchForce = 100;

    public bool pierce;
    Animator animator;


    public GameObject sfx;

    public float stickStrength;

    SpringJoint joint;

    public float dropAttackTreshold = 3;
    public float dropAttackBonus = 3;
    public void Start()
    {
        Mob = GetComponentInParent<Mob>();
        animator= GetComponentInParent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject.GetComponentInParent<Health>();
        if (!other) return;


        if (collision.relativeVelocity.y > dropAttackTreshold)
        {
            if (!joint)
            {
                print($"Stuck! {name} to {collision.gameObject}");
                joint = gameObject.AddComponent<SpringJoint>();
                joint.connectedBody = collision.rigidbody;
                joint.spring = 1000;
                joint.damper = 1;
                joint.breakForce = stickStrength;
            }

            print($"landed {name} on {collision.gameObject.name} at {collision.relativeVelocity.y}");
            other.Hurt(damage * dropAttackBonus, DamageKind.Melee, Mob);
        }
    }

    public void Attack()
    {
       
        var didAttack = false;
        if (Time.time - lastAttack > cooldown)
        {
            animator.SetTrigger("Attack");

            if (sfx)
            {
                Instantiate(sfx, transform.pos(), transform.rotation);
            }

            lastAttack = Time.time;
            var stuffToHurt = FindObjectsOfType<Health>().Where(x=>this.Distance(x)<attackRange).Select(x => x.GetComponent<Health>()).Where(x => x);
            foreach (var thing in stuffToHurt)
            {
                if (thing.Hurt(damage, DamageKind.Melee, Mob))
                {
                    Debug.DrawLine(transform.position, thing.transform.position, Color.red, cooldown);

                    thing.GetComponent<Rigidbody>().AddForce((Vector3.up + (thing.pos()- this.pos())).normalized * launchForce);

                    didAttack = true;
                    if (!pierce)
                        break;
                }
            }
        }
        
    }
}
