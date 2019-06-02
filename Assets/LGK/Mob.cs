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



	
}
