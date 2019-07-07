using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.UI;
using static InMan;

public class Player : MonoBehaviour
{

	public Controls Controls;

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

	public Mob Mob => GetComponent<Mob>();

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

	public LineRenderer trajectory;

	public IEnumerable<Norb> FindNearbyOwnedNorbs(float range)
	{
		return FindObjectsOfType<Norb>().Where(norb => norb.owner == this && norb.IsGrounded && this.Distance(norb) < range);
	}

	public float interactionRange = 3;


	public Image currentBuild;
	public BuildMenu buildMenu;

	public bool lockCamera;


	public Vector2 cmAngles;
	// Update is called once per frame
	void Update()
	{
		//Followers.RemoveWhere(U.Not);

		var mousePos = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay(mousePos);

		Mob over = null;

		if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask(Layers.Terrain)))
		{
			targeter.position = hit.point;
			targeter.rotation = transform.rotation;

			targeter.up = hit.normal;

			over = hit.collider.GetComponentInParent<Mob>();
		}

		lockCamera ^= CameraLock;

		Cursor.lockState = lockCamera ? CursorLockMode.None : CursorLockMode.Locked;

		var cam = FindObjectOfType<Cinemachine.CinemachineFreeLook>();


		cam.m_XAxis.m_InputAxisName = lockCamera ? "null" : "Mouse X";
		cam.m_YAxis.m_InputAxisName = lockCamera ? "null" : "Mouse Y";


		if (Kill && over?.Health)
		{
			var job = new MurderJob(over.Health);

			if (Team.JobManager.UnpublishJob(job))
			{
				print($"job {job.Name} is red");
			}
			else
			{
				print($"job {job.Name} is green");
				Team.JobManager.PublishJob(job);
			}
		}

		followPoint.transform.position = Rally ? targeter.position : transform.position - transform.forward;

		caller.gameObject.SetActive(Whistle);


		if (Input.GetKeyDown(KeyCode.N))
		{
			buildMenu.Next();
		}

		if (Build)
		{
			var tobuild = buildMenu.Current;
			if (balance >= tobuild.cost)
			{
				var spawnLocation = targeter.pos();

				var noob = Instantiate(tobuild, spawnLocation, targeter.rotation);
				//start
				balance -= noob.cost;
				noob.Team = Team;

				var village = gameObject.Find<VillageController>(100).Closest(gameObject);
				village.IBuilt(Team, noob);
				noob.Place(this);
			}

		}


		if (Melee)
		{
			weapon.Attack();
		}

		if (Harvest)
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

		{
			var targetPos = targeter.position;
			var startPos = transform.position + 2 * Vector3.up;

			var launchVel = PhysicsUtils.ComputeThrow(startPos, targetPos, 3 * LaunchVel, false);

			startPos += launchVel.normalized;

			launchVel = PhysicsUtils.ComputeThrow(startPos, targetPos, 3 * LaunchVel, false);

			if (!float.IsNaN(launchVel.sqrMagnitude))
			{

				if (Fireball)
				{
					if (!float.IsNaN(launchVel.x))
					{
						var fireball = Instantiate(fireballPrefab, startPos, Quaternion.identity);
						fireball.GetComponent<FireBall>().by = health;
						fireball.velocity = launchVel;
					}
				}



				var deltat = LaunchVel / Physics.gravity.magnitude / trajectory.positionCount;

				var pos = startPos;

				var vel = launchVel;



				var poses = new Vector3[trajectory.positionCount];
				for (int i = 0; i < trajectory.positionCount; i++)
				{
					var t = deltat * i;
					pos += vel * deltat;
					vel += deltat * Physics.gravity;
					poses[i] = pos;
				}
				trajectory.SetPositions(poses);
			}
		}

		if (Throw && Followers.Any())
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
		if (Summon)
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

		if (Whistle)
		{
			var nearby = U.Find<Mob>(targeter.pos(), maxCallRange).Where(norb => norb.Team == Team && norb.IsGrounded);
			foreach (var norb in nearby)
			{
				norb.SwitchBehavior<FollowBehavior>(x=>x.following = this);
			}
		}
	}
}
