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
        health.OnHurt += Health_OnHurt;
    }

    private void Health_OnHurt(Health by)
    {
        var followers = Followers.ToArray();
        if (followers.Any())
        {
            var savemesenpi = followers.Random();
            savemesenpi.SetJob(new Job(JobKind.Attack, by.gameObject));

        }
    }

    public float LaunchVel = 3;

    public float NorbCost = 5;

    public float norbGrabRange = 2;
    public float maxCallRange = 5;
    public IEnumerable<Norb> Followers => FindNearbyOwnedNorbs(norbGrabRange);


    public IEnumerable<Norb> FindNearbyOwnedNorbs(float range)
    {
        return FindObjectsOfType<Norb>().Where(norb => norb.owner == this && norb.IsGrounded && this.Distance(norb) < range);
    }

    // Update is called once per frame
    void Update()
    {
        var doThrow = Input.GetButtonDown("Fire1");
        var doSummon = Input.GetButtonDown("Fire2");
        var doWhistle = Input.GetKey(KeyCode.R);


        if (doThrow)
        {

            var noob = Followers.MinBy(n => this.Distance(n));
            if (noob)
            {
                noob.transform.position = transform.position + transform.forward + Vector3.up;
                noob.GetComponent<Rigidbody>().velocity = (transform.forward + Vector3.up).normalized * LaunchVel;
                noob.GetComponent<ThirdPersonCharacter>().IsGrounded = false;
                noob.SetJob(Job.Idle);
            }
        }
        if (doSummon)
        {
            health.Hurt(NorbCost, DamageKind.Sacrifice, null, health);
            var noob = Instantiate(NorbPrefab, transform.position + transform.forward+Vector3.up, Quaternion.identity);
            noob.GetComponent<Health>().team = health.team;
            noob.owner = this;
        }

        if (doWhistle)
        {
            foreach(var norb in FindNearbyOwnedNorbs(maxCallRange))
            {
                norb.FollowPlayer();
            }
        }

    }
}
