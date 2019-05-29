using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// HATRED POINTS
/// HINDERING POINTS
/// HEADACHE PAIN
/// HINDERING POINTS
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
    

    private float lastHurt = 0;
	public float killHealthBonus = 0.15f;

	public Team team;

	public void Heal(float value)
	{
		CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + value);
	}

	public void Hurt(float value, DamageKind kind, Team by)
	{
		if (!Team.Fighting(by,team))
			return;

        lastHurt = Time.time;
        print($"{by?.name} attacked {gameObject?.name} for {value} {kind}");

		
		CurrentHealth -= value;
		if(CurrentHealth < 0)
		{
			//if (by)
			//{
			//	by.CurrentHealth += MaxHealth * killHealthBonus;
			//}
			if(ragdoll)
				Instantiate(ragdoll, transform.position, transform.rotation);

			Destroy(gameObject);
		}
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
	Blunt,
	Magic
}