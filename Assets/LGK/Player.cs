using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public double balance
	{
		get => Team.Balance;
		set => Team.Balance = value;
	}
	public Scrollbar foodBar;


	public HashSet<Mob> Followers = new HashSet<Mob>();

	/// <summary>
	/// Hate Points
	/// </summary>
	public Health Health
	{
		get
		{
			if (health)
				return health;

			return health = GetComponent<Health>();
		}
	}
	private Health health;

	public Team Team => health.team;
	public Swarm squad;
	public MeleeWeapon weapon;
	public Norb NorbPrefab;

	public Transform targeter;
	public ParticleSystem caller;

	// Start is called before the first frame update
	void Start()
	{
		weapon = weapon ?? GetComponentInChildren<MeleeWeapon>();
		squad = GetComponent<Swarm>();
		Health.OnHurt += Health_OnHurt;

		var shape = caller.shape;
		shape.radius = maxCallRange;
	}

	private void Health_OnHurt(Health by)
	{
		var followers = Followers.ToArray();
		if (followers.Any())
		{
			var savemesenpi = followers.Random();

			//TODO fix
			//savemesenpi.SetJob(new Job(JobKind.Attack, by.gameObject));

		}
	}

	public float LaunchVel = 3;

	public float NorbCost = 5;

	public float norbGrabRange = 2;
	public float maxCallRange = 5;
	public GameObject followPoint;

	public Rigidbody fireballPrefab;



	public IEnumerable<Norb> FindNearbyOwnedNorbs(float range)
	{
		return FindObjectsOfType<Norb>().Where(norb => norb.owner == this && norb.IsGrounded && this.Distance(norb) < range);
	}

	public float interactionRange = 3;


	public Image currentBuild;
	public BuildMenu buildMenu;

	// Update is called once per frame
	void Update()
	{
		//Followers.RemoveWhere(U.Not);

		var mousePos = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay(mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			targeter.position = hit.point;
		}
		else
		{

		}


		var doThrow = Input.GetButtonDown("Fire1");
		var doSummon = Input.GetKeyDown(KeyCode.Y);
		var doFireball = Input.GetButtonDown("Fire2");
		var doHarvest = Input.GetKeyDown(KeyCode.H);
		var doMelee = Input.GetKeyDown(KeyCode.M);
		var doBuild = Input.GetKeyDown(KeyCode.B);




		var doLocationController = Input.GetKey(KeyCode.Mouse2);
		var doWhistle = Input.GetKey(KeyCode.R);

		followPoint.transform.position = doLocationController ? targeter.position : transform.position - transform.forward;

		caller.gameObject.SetActive(doWhistle);


		if (Input.GetKeyDown(KeyCode.N))
		{
			buildMenu.Next();
		}

		if (doBuild)
		{
			var tobuild = buildMenu.Current;
			if (balance >= tobuild.cost)
			{
				var spawnLocation = targeter.pos();

				var noob = Instantiate(tobuild, spawnLocation, targeter.rotation);

				balance -= noob.cost;
				var village = gameObject.Find<VillageController>(100).Closest(gameObject);
				village.IBuilt(Team, noob);
				noob.Place(this);
			}

		}


		if (doMelee)
		{
			weapon.Attack();
		}

		if (doHarvest)
		{
			var offering = gameObject.Find<Offering>(interactionRange).Closest(gameObject);
			if (offering)
			{
				balance += offering.curret;
				offering.curret = 0;
			}
			else
			{

				var storeroom = gameObject.Find<Storeroom>(interactionRange).Closest(gameObject);

				if (storeroom)
				{
					storeroom.Rob(Team);
				}
			}
		}

		if (doFireball)
		{
			var targetPos = targeter.position;
			var startPos = transform.position + 2 * Vector3.up;

			var launchVel = PhysicsUtils.ComputeThrow(startPos, targetPos, 3 * LaunchVel, false);

			startPos += launchVel.normalized;

			launchVel = PhysicsUtils.ComputeThrow(startPos, targetPos, 3 * LaunchVel, false);

			if (!float.IsNaN(launchVel.x))
			{
				var fireball = Instantiate(fireballPrefab, startPos, Quaternion.identity);
				fireball.GetComponent<FireBall>().by = health;
				fireball.velocity = launchVel;
			}
		}

		if (doThrow && Followers.Any())
		{

			var noob = Followers.MinBy(n => this.Distance(n));

			if (noob && noob.Distance(this) < norbGrabRange)
			{
				var startPos = transform.position + transform.forward + Vector3.up;
				var targetPos = targeter.position;



				var launchVel = PhysicsUtils.ComputeThrow(startPos, targetPos, LaunchVel);

				noob.transform.position = startPos;
				noob.Throw(launchVel);
			}
		}
		if (doSummon)
		{
			//Health.Hurt(NorbCost, DamageKind.Sacrifice);
			//var noob = Instantiate(NorbPrefab, transform.position + transform.forward+Vector3.up, Quaternion.identity);
			//noob.GetComponent<Health>().team = Health.team;
			//noob.owner = this;

			var alter = gameObject
				.Find<BloodAlter>(interactionRange)
				.MinBy(gameObject.Distance);
			if (alter)
			{
				alter.Spawn(this);
			}
		}

		if (doWhistle)
		{
			var nearby = U.Find<Mob>(targeter.pos(), maxCallRange).Where(norb=> norb.Team==Team && norb.IsGrounded);
			foreach (var norb in nearby)
			{
				norb.SwitchBehavior<FollowBehavior>();
			}
		}
	}


}
