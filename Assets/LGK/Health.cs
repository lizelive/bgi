using System;
using UnityEngine;

using System.Linq;
/// <summary>
/// HATRED POINTS
/// HINDERING POINTS
/// HEADACHE PAIN
/// HERO POINTS
/// </summary>
public class Health : MonoBehaviour
{
    public float MaxHealth = 100;
    public float CurrentHealth = 100;
    public float HitCooldown = 1;
    public float RegenCooldown = float.PositiveInfinity;
    public float RegenSpeed = 0;
    public GameObject ragdoll;
    public ParticleSystem deathParticle;


    // i guess this is a thing?
    public float Stagger;
    public float StaggerDecay;

    public DamageKind[] immunities;

    private float lastHurt = 0;
    public float hurtBonus = 1f;

    public Team team;

    public event Action<Health> OnHurt;
    public event Action<Health> OnDie;



    public Func<Health, bool> InRange(float range) => (x) => InRange(range, x);
    public bool InRange(float range, Health other)
    {
        return this.Distance(other) < range + other.radius + radius;
    }

    public void Heal(float value)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + value);
    }

    public bool Hurt(float value, DamageKind kind, Health by = null, Team byTeam = null, bool allowFriendlyFire = false, bool ignoreCooldown = false)
    {
        if (kind == DamageKind.None)
        {
            Debug.LogWarning("That's not very effective.");
            return false;
        }

        byTeam = byTeam ?? by?.team;
        if (by == this || (!Team.Fighting(byTeam, team) && !allowFriendlyFire) || immunities.Contains(kind))
            return false;


        if (!ignoreCooldown && Time.time - lastHurt < HitCooldown)
        {
            return false;
        }
        //print($"{gameObject} was hurt for {value} {kind} by {hurter} but {immunities}");



        lastHurt = Time.time;


        var pain = Mathf.Min(CurrentHealth, value);

        if (byTeam)
        {
            byTeam.Balance += Mathf.Clamp(pain, 0, float.PositiveInfinity);
        }

        Stagger += pain / MaxHealth;
        CurrentHealth -= pain;
        team?.ReportMurder(byTeam, pain);
        if (CurrentHealth <= 0)
        {
            //if (by)
            //{
            //	by.CurrentHealth += MaxHealth * killHealthBonus;
            //}

            Die(by);
        }

        OnHurt?.Invoke(by);
        return true;
    }

    private bool dead = false;

    void Die(Health by)
    {
        dead = true;



        if (ragdoll)
            Instantiate(ragdoll, transform.position, transform.rotation);
        if (deathParticle)
        {
            var dp = Instantiate(deathParticle, transform.position, Quaternion.identity);
            var shape = dp.shape;
            var smr = shape.skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            print(smr);
        }
        OnDie?.Invoke(by);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {

        if (!team)
            return;

        if (!team.Unregister(this))
        {
            Debug.LogWarning($"{name} on {team} died without registing?");
        }
    }

    public Vector3 CenterPoint => transform.TransformPoint(centerOffset);
    public Vector3 centerOffset;
    public float radius;
    // Start is called before the first frame update
    void Start()
    {
        centerOffset = GetComponentInChildren<Rigidbody>()?.centerOfMass
            ?? GetComponentInChildren<Collider>()?.bounds.center ??
            Vector3.zero;



        radius = GetComponentInChildren<Collider>()?.bounds.size.Max() ?? 0;
    }

    private void Awake()
    {
        //ha lol hotswap this
        team?.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastHurt > RegenCooldown)
            Heal(RegenSpeed * Time.deltaTime);


        // not sure but things feel too strong
        // can be used to stun enemies or something
        // health just tracks it

        // should be less than one unless your really messed something up
        Stagger -= Mathf.Min(Stagger, StaggerDecay * Time.deltaTime);



    }
}

public enum DamageKind
{
    None,
    Generic,
    Fire,
    Melee,
    Explosion,
    Magic,
    Water,
    Sacrifice,
    Step,
    Fall
}