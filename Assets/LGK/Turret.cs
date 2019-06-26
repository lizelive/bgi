using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour
{
	public Transform barrel;
	public float viewRange =100;
	public float maxTurnSpeed = 6;
	public bool aimAhead =true;

	public float damage = 3;

	public float bulletVel = 80;

	public Health target;

	Health health;

	private void Start()
	{
		health = GetComponent<Health>();
	}

	void OnParticleCollision(GameObject other)
	{

		var health = other.GetComponent<Health>();
		if(health)
		health.Hurt(damage, DamageKind.Magic, health);
	}
	Vector3 lastTargetPos;
	// Update is called once per frame
	void Update()
    {
		if (!target || this.Distance(target)>viewRange)
		{
			target = gameObject.Find<Health>(viewRange).Where(health.team.Fighting).Closest(gameObject);
			lastTargetPos = target?.pos() ?? Vector3.zero;
		}



		
		// delay on notice
		if (target)
		{
			var targetPos = target.CenterPoint;

			if (aimAhead)
			{
				var vel = (lastTargetPos - targetPos)/Time.deltaTime;

				var travelTime = this.Distance(targetPos) / bulletVel;

				targetPos += vel * travelTime;
			}

			var targetRot = Quaternion.LookRotation(targetPos - barrel.pos());
			var angle = Quaternion.Angle(barrel.rotation, targetRot);
			barrel.rotation = Quaternion.RotateTowards(barrel.rotation, targetRot, maxTurnSpeed * Time.deltaTime);

			lastTargetPos = target.CenterPoint;
		}
	}
}
