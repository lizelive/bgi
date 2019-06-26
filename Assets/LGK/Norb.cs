using System;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;


public class Norb : MonoBehaviour
{
    public Player owner;

    Team team => health.team;
    

    public Text debugText;
    public Swarm swarm;

    public Rigidbody holding;
    public float maxFollowRange = 5;
    public Transform holdSpot;
    MeleeWeapon weapon;

    public Rigidbody test;

    public Job job;
    internal Health health;


    public float ownerHealRate = 0.1f;
    public float ownerHealRange = 3;

    public bool IsGrounded
    {
        get => GetComponent<Mob>().IsGrounded;
        set => GetComponent<Mob>().IsGrounded = value;
    }

    private float lastSeek;
    public bool CanDrown;
    
    // Start is called before the first frame update
    void Start()
    {
        
        health = GetComponent<Health>();
        weapon = GetComponentInChildren<MeleeWeapon>();
        SetJob(Job.Seek);


        health.OnHurt += OnHurt;

        if(health.team)
        foreach (var mr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (var mat in mr.materials)
            {
                
                mat.color = health.team.color;

            }
        }

    }

    void OnHurt(Health by)
    {
        Drop();
        if (by)
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
        else
        {
            return false;
        }
    }
    bool Pickup(GameObject thing)
    {
        return Pickup(thing.GetComponent<Rigidbody>());
    }

    bool Pickup(Rigidbody thing)
    {
        if (holding || !thing || thing.transform.parent)
            return false;
        holding = thing;
        thing.isKinematic = true;
        var height = thing.transform.position.y - thing.ClosestPointOnBounds(thing.transform.position + 10 * Vector3.down).y;
        thing.transform.parent = holdSpot;
        thing.GetComponentInChildren<Collider>().enabled = false;
        thing.transform.localPosition = Vector3.up * height;
        thing.transform.localRotation = Quaternion.identity;
        return true;
    }


    public float attackRange = 1.1f;

    public float SeekRange = 5;

    public void SetJob(Job job)
    {
        this.job = job;
        switch (job.Kind)
        {
            case JobKind.Pickup:
                if (!holding)
                    Mob.SetTarget(job.target.transform);
                break;
            case JobKind.Goto:
            
            case JobKind.Attack:
            case JobKind.Follow:
                Mob.SetTarget(job.target.transform);
                break;
            case JobKind.Idle:
            case JobKind.Seek:
            case JobKind.Ragdoll:
                Mob.TargetClear();

                break;

            default:
                break;
        }
    }


    Job FollowPlayerJob => owner ? new Job { Kind = JobKind.Follow, target = owner.followPoint } : Job.Seek;

    public Mob Mob => GetComponent<Mob>();

    public void FollowPlayer()
    {
        SetJob(FollowPlayerJob);
    }


    // Update is called once per frame
    void Update()
    {
        if (debugText)
        {
            debugText.text = $"{Mob?.Team?.name} {job?.Kind} {health?.CurrentHealth}";
        }

        
        if (owner && this.Distance(owner) < ownerHealRange)
        {
            owner.Health.Heal(Time.deltaTime * ownerHealRate);
        }


        //if (job == null) SetJob(Nextjob);

        if (job == null)
        {
            print("how is my job null? im just going to idle");
            SetJob(Job.Idle);
            return;
        }

        if (job.target)
        {
            var distToTarget = gameObject.Distance(job.target);
            if (distToTarget > maxFollowRange)
            {
                SetJob(Job.Seek);
                return;
            }
        }

        //print($"{name} is working on {job.Kind}");

        var atTarget = Mob.AtTarget;


        if(job.Kind != JobKind.Ragdoll && job.Kind != JobKind.Drown)
        if (Time.time - lastSeek > 0.1)
        {
            var newMission = NextJob();
            if (newMission != null)
                SetJob(newMission);
        }

        switch (job.Kind)
        {
            case JobKind.Drown:
                health.Hurt(1 * Time.deltaTime, DamageKind.Water);
                break;
            case JobKind.Ragdoll:
                if (Mob.IsGrounded)
                    SetJob(Job.Seek);
                break;
            case JobKind.Goto:
    

                    if (atTarget)
                {
                    SetJob(Job.Seek);
                }
                break;
            case JobKind.Pickup:

                if (job.target.transform.parent||holding)
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
                if (!job.target)
                    SetJob(Job.Seek); break;
            case JobKind.Seek:

                SetJob(NextJob() ?? Job.Idle);

                // look for nerby things to attack or whatever
                break;
            default:
                Debug.LogError("Invalid job type");
                break;
        }
    }

    public void Throw(Vector3 launchVel)
    {
        Mob.IsGrounded = false;
        SetJob(Job.Thrown);
        GetComponent<Rigidbody>().velocity = launchVel;
        Mob.TargetClear();
    }

    public virtual Job NextJob()
    {
        lastSeek = Time.time;
        var thingToKill = FindObjectsOfType<Health>().FirstOrDefault(h => Team.Fighting(team, h.team) && Vector3.Distance(h.transform.position, transform.position) < SeekRange);
        if (thingToKill)
        {
            return (new Job(JobKind.Attack, thingToKill.gameObject));
        }
        var thingToCarry = FindObjectsOfType<PickupMePls>().FirstOrDefault(h =>
        this.Distance(h) < SeekRange && !h.transform.parent);
        if (thingToCarry)
        {
            return (new Job(JobKind.Pickup, thingToCarry.gameObject));
        }


        if (owner && this.Distance(owner) < SeekRange)
        {
            return FollowPlayerJob;
        }

        return null;
    }
}


public enum JobKind
{
    Idle,
    Goto,
    Pickup,
    Attack,
    Seek,
    Follow,
    Ragdoll,
    Drown
}

public class Job
{
    public static Job Idle => new Job { Kind = JobKind.Idle };
    public static Job Seek => new Job { Kind = JobKind.Seek };

    public static Job Thrown => new Job(JobKind.Ragdoll);

    public static Job Drown => new Job(JobKind.Drown);

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