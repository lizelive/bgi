using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Hate Points
    /// </summary>
    public Health health;
    public Swarm squad;
    public MeleeWeapon weapon;
    public Norb NorbPrefab;
    // Start is called before the first frame update
    void Start()
    {
        weapon = weapon ?? GetComponentInChildren<MeleeWeapon>();
        health = GetComponent<Health>();
        squad = GetComponent<Swarm>();
    }

    public float LaunchVel = 3;

    public float NorbCost = 5;

    // Update is called once per frame
    void Update()
    {
        var doThrow = Input.GetButtonDown("Fire1");
        var doSummon = Input.GetButtonDown("Fire2");


        if (doThrow)
        {

            var noob = FindObjectsOfType<Norb>().Where(norb => norb.owner == this && norb.IsGrounded).MinBy(n => this.Distance(n));
            if (noob)
            {
                noob.transform.position = transform.position + transform.forward + Vector3.up;
                noob.GetComponent<Rigidbody>().velocity = (transform.forward + Vector3.up).normalized * LaunchVel;
                noob.GetComponent<ThirdPersonCharacter>().IsGrounded = false;
            }
        }
        if (doSummon)
        {
            health.Hurt(NorbCost, DamageKind.Sacrifice, null, health);
            var noob = Instantiate(NorbPrefab, transform.position + transform.forward+Vector3.up, Quaternion.identity);
            noob.GetComponent<Health>().team = health.team;
            noob.owner = this;
        }

    }
}
