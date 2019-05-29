using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]

public class Mob : MonoBehaviour
{
	public float Cost = 1;
	public string AltText;


	public float ViewRange = 100;
	public float ViewAngle = 180;
	public float CloseRange = 180;
	public float AttackRange = 3;

	public GameObject ragdoll;

	public float Damage = 1;
	NavMeshAgent agent;
	Animator animator;
	public Health health;

	public float targetMoveThreshold = 1f;
	public TargetDesignator target;

	enum MobType
	{
		Necromancer,
		Melee,
		Ranged
	}
    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponentInChildren<Animator>();
		agent = GetComponent<NavMeshAgent>();
		health = GetComponent<Health>();
    }
	public float forward;
	public List<Team> LookingFor;

	public Team Team => health.team; 

	public float AttackCooldown = 1f;

	private Vector3 targetPos;

	public TargetKind kindOfWork = TargetKind.Attack;

	
	void OnDie()
	{
	}

	public GameObject zombie;
	public float lastAttackTime;
	void Attack()
	{
		var now = Time.time;
		if (now - lastAttackTime < AttackCooldown)
			return;


		bool didAttack = false;

		var near = FindObjectsOfType<TargetDesignator>().Where(h => h.team != this.Team && Vector3.Distance(transform.position, h.transform.position) < AttackRange).ToArray();


		print(near.Length);
		foreach (var posible in near)
		{
			if (posible.Kind != kindOfWork)
				continue;
			if (posible.Kind == TargetKind.Attack)
			{
				var health = posible.GetComponent<Health>();
				if (health && health != this.health)
				{
					didAttack = true;
					health.Hurt(Damage, DamageKind.Blunt, this);
				}
			}
			else if(posible.Kind == TargetKind.Revive)
			{
				print("Rise!!!");
				Instantiate(zombie, posible.Pos, Quaternion.identity);
				Destroy(posible.gameObject);
				didAttack = true;
				break;
			}
		}

		if (didAttack)
		{
			lastAttackTime = now;
			animator.SetTrigger("Attack");
		}

		
	}


	public float RotationSpeed = 360;
	public bool updateTargetEveryFrame = true;
    // Update is called once per frame
    void Update()
    {
			Attack();
		var pos = transform.position;

		if (!target || updateTargetEveryFrame)
		{
			TargetDesignator newTarget = null;

			var possibleTargets = FindObjectsOfType<TargetDesignator>();

			float targetWeightedDistance = float.PositiveInfinity;

			foreach (var possible in possibleTargets)
			{
				if (!LookingFor.Contains(possible.team) || possible.Kind != kindOfWork)
					continue;
				var dist = Vector3.Distance(transform.position, possible.transform.position);

				var dirToChest = Vector3.Normalize(transform.position - possible.transform.position);

				var angle = Vector3.Angle(transform.forward, dirToChest);

				if (dist > ViewRange || !(angle < ViewAngle || dist < CloseRange))
					continue;



				if (possible.NeedLineOfSight && Physics.Raycast(transform.position, dirToChest, out RaycastHit hit) && hit.transform.GetComponent<TargetDesignator>() != possible)
					continue;
				var weightedDist = dist / possible.Weight;
				// the chest is good. check if it's the closests
				if (weightedDist < targetWeightedDistance)
				{
					newTarget = possible;
					targetWeightedDistance = dist;
				}

			}
			target = newTarget;

		}

		float turn = 0;

		if (target ) {
			float targetDistance = Vector3.Distance(transform.position, target.Pos);

			if(Vector3.Distance(targetPos, target.Pos) > targetMoveThreshold)
			{
				targetPos = target.Pos;
			}

			if(target.Kind == TargetKind.Attack || target.Kind == TargetKind.Revive)
			{
				// too close ish.
				if (targetDistance < AttackRange)
				{
					//targetPos = transform.position;
					Attack();
				}
			}

			var lookAtPos = target.Pos;

			lookAtPos.y = pos.y;

			var lookDir = lookAtPos - pos;

			var lookRotation = Quaternion.LookRotation(lookDir);
			turn = Vector3.SignedAngle(transform.forward, lookDir, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, RotationSpeed*Time.deltaTime);

			agent.SetDestination(targetPos);

		}
		var vel = agent.desiredVelocity.magnitude;
		
		animator.SetFloat("Forward", vel);
		animator.SetFloat("Turn", turn);
	}
}
