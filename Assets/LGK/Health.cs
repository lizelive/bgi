using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public DamageKind immunities;

    private float lastHurt = 0;
	public float killHealthBonus = 0.15f;

	public Team team;

    public event Action<Health> OnHurt;

	public void Heal(float value)
	{
		CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + value);
	}

    public bool Hurt(float value, DamageKind kind, Health hurter = null)
    {
        return Hurt(value, kind, hurter?.team, hurter);
    }



    public bool Hurt(float value, DamageKind kind, Team by, Health hurter = null)
	{
        print($"{gameObject} was hurt for {value} {kind} by {hurter} but {immunities}");
        if (!Team.Fighting(by, team) || immunities==kind)
			return false;



        lastHurt = Time.time;
		
		CurrentHealth -= value;
		if(CurrentHealth <= 0)
		{
			//if (by)
			//{
			//	by.CurrentHealth += MaxHealth * killHealthBonus;
			//}
			
            Die();
		}

        OnHurt?.Invoke(hurter);
        return true;
    }
    void Die()
    {
        if (ragdoll)
            Instantiate(ragdoll, transform.position, transform.rotation);
        if (deathParticle)
        {
            var dp = Instantiate(deathParticle, transform.position, Quaternion.identity);
            var shape = dp.shape;
            var smr = shape.skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            print(smr);
        }
        Destroy(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastHurt > RegenCooldown)
            Heal(RegenSpeed * Time.deltaTime);
        
    }
}

public enum DamageKind
{
	Generic,
	Fire,
	Melee,
    Explosion,
    Magic,
    Water,
    Sacrifice
}