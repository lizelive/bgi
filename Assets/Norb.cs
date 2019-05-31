using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(AIMovement))]
public class Norb : MonoBehaviour
{
    public Player owner;

    Team team => health.team;

    AIMovement movement;                         // target to aim for


    public Swarm swarm;

    public Rigidbody holding;

    public Transform holdSpot;
    MeleeWeapon weapon;

    public Rigidbody test;

    public Job job;
    internal Health health;


    public bool IsGrounded {
        get => GetComponent<ThirdPersonCharacter>().IsGrounded;
        set => GetComponent<ThirdPersonCharacter>().IsGrounded = value;
            }

    private float lastSeek;
    // Start is called before the first frame update
    void Start()
    {

        movement = GetComponent<AIMovement>();
        health = GetComponent<Health>();
        weapon = GetComponentInChildren<MeleeWeapon> ();
        SetJob(Job.Seek);
        health.OnHurt += OnHurt;

    }


    void OnHurt(Health by)
    {
        if(by)
        SetJob(new Job(JobKind.Attack, by.gameObject));
    }

    bool Drop()
    {
        
        if (holding)
        {
            holding.transform.parent = null;
            holding.isKinematic = false;
            holding.GetComponent<Collider>().enabled = true;
            return true;
        }
        else {
            return false;
        }
    }
    bool Pickup(GameObject thing)
    {
        return Pickup(thing.GetComponent<Rigidbody>());
    }

    bool Pickup(Rigidbody thing)
    {
        if (holding||!thing||thing.transform.parent)
            return false;
        holding = thing;
        thing.isKinematic = true;
        var height = thing.transform.position.y - thing.ClosestPointOnBounds(thing.transform.position + 10 * Vector3.down).y;
        thing.transform.parent = holdSpot;
        thing.GetComponent<Collider>().enabled = false;
        thing.transform.localPosition = Vector3.up * height;
        thing.transform.localRotation = Quaternion.identity;
        return true;
    }


    public float attackRange = 1.1f;

    public float SeekRange = 5;

    void SetJob(Job job)
    {
        this.job = job;
        switch (job.Kind)
        {

            case JobKind.Goto:
            case JobKind.Pickup:
            case JobKind.Attack:
            case JobKind.Follow:
                movement.SetTarget(job.target.transform);
                break;
            case JobKind.Idle:
                break;
            case JobKind.Seek:
                // 1st 

                break;

            default:
                break;
        }
    }

    void FollowPlayer()
    {

        if(owner)
        SetJob(new Job { Kind = JobKind.Follow, target = owner.gameObject });
        else
        SetJob(Job.Seek);
    }


    // Update is called once per frame
    void Update()
    {

        print($"{name} is working on {job.Kind}");
        //if (job == null) SetJob(Nextjob);

        if (job == null)
        {
            print("how is my job null? im just going to idle");
            SetJob(Job.Idle);
            return;
        }
            

        var atTarget = movement.AtTarget;
        switch (job.Kind)
        {
            case JobKind.Goto:
                if (atTarget)
                {
                    SetJob(Job.Seek);
                }
                break;
            case JobKind.Pickup:

                if(job.target.transform.parent)
                    SetJob(Job.Seek);
                else
                if (atTarget)
                {
                    print("going to lift");
                    if (Pickup(job.target))
                    {
                        FollowPlayer();
                    }
                    else
                    {
                        print("failed to lift");
                        SetJob(Job.Seek);
                    }
                }
                break;
            case JobKind.Idle:
                if(Time.time - lastSeek > 1)
                    SetJob(Job.Seek);
                // when hurt defend self
                break;
            case JobKind.Attack:
                if (!job.target)
                {
                    SetJob(Job.Seek);
                }
                if (atTarget)
                {
                    weapon.Attack();
                }
                break;
            case JobKind.Follow:
                break;
            case JobKind.Seek:
                lastSeek = Time.time;
                var thingToKill = FindObjectsOfType<Health>().FirstOrDefault(h => Team.Fighting(team, h.team)
&& Vector3.Distance(h.transform.position, transform.position) < SeekRange);
                if (thingToKill)
                {
                    SetJob(new Job(JobKind.Attack, thingToKill.gameObject));
                    break;
                }
                var thingToCarry = FindObjectsOfType<PickupMePls>().FirstOrDefault(h =>
                this.Distance(h) < SeekRange && !h.transform.parent);
                if (thingToCarry)
                {
                    SetJob(new Job(JobKind.Pickup, thingToCarry.gameObject));
                    break;
                }


                if (owner && this.Distance(owner) < SeekRange)
                {
                    FollowPlayer();
                    break;
                }

                SetJob(Job.Idle);

                // look for nerby things to attack or whatever
                break;
            default:
                Debug.LogError("Invalid job type");
                break;
        }
    }
}


public enum JobKind
{
    Idle,
    Goto,
    Pickup,
    Attack,
    Seek,
    Follow
}

public class Job
{
    public static Job Idle => new Job { Kind = JobKind.Idle };
    public static Job Seek => new Job { Kind = JobKind.Seek };
    public float priority;
    public JobKind Kind;
    public GameObject target;

    public Job()
    {
    }

    public Job(JobKind kind)
    {
        this.Kind = kind;
    }

    public Job(JobKind kind, GameObject target) : this(kind)
    {
        this.target = target;
    }
}