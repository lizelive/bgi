using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class Mob : MonoBehaviour
{
	public float cost = 1;
    public bool CanSee(Vector3 pos)
    {
        var dir = pos - this.pos();
        var dist = dir.magnitude;
        dir.Normalize();

        if (
            Physics.Raycast(this.head.position + dir, dir, out var hit)
            //&& hit.distance <= Me.ViewRange
            && (Mathf.Abs(dist - hit.distance) < 2 || hit.distance > dist))
        {
            return true;
        }
        return false;
    }

    public bool CanSee(MonoBehaviour IWannaKill) => CanSee(IWannaKill.pos());

	public bool physicsMovement;
    public AiBehavior[] Behaviors = new AiBehavior[0];


    public AiBehavior ActiveBehavior;

    public AiBehavior SwitchBehavior(AiBehavior next)
    {
		if (ActiveBehavior == next)
			return ActiveBehavior;

        if (ActiveBehavior)
        {
            if (!ActiveBehavior.OnEnd())
            {
                return null;
            }
            else
                ActiveBehavior = null;
        }
		if (next?.OnBegin() ?? true)
		{
			ActiveBehavior = next;

			return ActiveBehavior;
		}
		else return null;

    }

    internal void SetVelocity(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
    }

    public AiBehavior SwitchBehavior()
    {
        return SwitchBehavior(Behaviors.ToDictionary(b => b, b => b.CurrentPriority).WeightedRandom());
    }


	
	public T SwitchBehavior<T>(Action<T> predo) where T : AiBehavior
	{
		var next = GetComponent<T>();
		if (!next)
		{
			Debug.LogError($"Missing behavior {typeof(T).Name}");
		}
		predo(next);
		return SwitchBehavior(next) as T;

	}


	public T SwitchBehavior<T>() where T : AiBehavior
    {
		return SwitchBehavior<T>(x => { });

	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position
		if (Team)
		{
			Gizmos.color = Team.color;
			UnityEditor.Handles.color = Team.color;
		}
			if (!head)
			print(name);
		else
		UnityEditor.Handles.Label(head.position, ActiveBehavior?.GetType()?.Name);
	}
#endif

	public IEnumerable<Mob> Nearby => gameObject.Find<Mob>(ViewRange);
    public IEnumerable<Mob> NearbyEnemies => Nearby.Where(this.Team.Fighting);

    Rigidbody carrying;


    VoxelNavAgent agent;
    public Animator Animator { get; private set; }

    new Rigidbody rigidbody;
    private Health _health;
    public Health Health
    {
        get
        {
            if (!_health)
                _health = GetComponent<Health>();
            return _health;
        }
    }

    public float ViewRange = 10;
    public float maxSpeed = 1;
    public Team Team {
		get => Health.team;
		set => Health.team = value;
	}

    private bool m_IsGrounded;
    private Vector3 m_GroundNormal;
    public float GroundCheckDistance = 1f;
    public bool IsGrounded { get => m_IsGrounded; internal set { m_IsGrounded = false; } }
    public Vector3 targetpos;
    public float targetDistace;

    public UnityEngine.Transform target;                                    // target to aim for

    public float targetDistanceGoal = 1;

    Vector3 lastTargetPos;
    public float targetPosMoveThreshold = 1;

    float lastYaw;


    public enum State
    {
        Normal,
        Ragdoll
    }

    public State state;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        Animator.applyRootMotion = false;
        agent = GetComponent<VoxelNavAgent>();
        head = head ?? transform;
        Behaviors = GetComponents<AiBehavior>();

        if (agent)
        {
            var newtonIsALie = !physicsMovement;
            agent.updatePosition = newtonIsALie;
        }
        rigidbody = GetComponent<Rigidbody>();

		rigidbody.isKinematic = !physicsMovement;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        SwitchBehavior();
    }

    public float stepHeight = 0.2f;
    public float notAStepHeight = 0.05f;
    public float jumpVelocity = 4;

    public void Move(Vector3 targetVel, float turn = 0)
    {
        //targetVel = transform.TransformVector(targetVel);
        if (IsGrounded)
        {

            var checkPosLower = transform.position + targetVel.normalized;

            var checkPos = checkPosLower + Vector3.up * stepHeight;


            Debug.DrawLine(transform.position, checkPos, Color.green);


            RaycastHit hit;
            if (

                Physics.Raycast(
                ray: new Ray(checkPos, Vector3.down),
                maxDistance: stepHeight- notAStepHeight,
                layerMask: LayerMask.GetMask("Terrain"),
                hitInfo: out hit) // should be true most of the time
                ||
                Physics.Raycast(
                ray: new Ray(checkPosLower, Vector3.up),
                maxDistance: stepHeight- notAStepHeight,
                layerMask: LayerMask.GetMask("Terrain"),
                hitInfo: out hit) // fuck sketchup


                )
            {
                //print("step detected!");


                Debug.DrawLine(checkPos, hit.point);


                //Todo proportanal jumps for the lols
                targetVel.y += jumpVelocity;


                //targetVel = PhysicsUtils.ComputeThrow(transform.pos(), hit.point, jumpVelocity);

                //transform.position = rayhit.point;
            }
            else
            {
                targetVel.y = rigidbody.velocity.y;
            }


            if (state == State.Normal)
                rigidbody.velocity = targetVel;



            //rigidbody.angularVelocity = Vector3.up * turn;
        }
    }






    bool CheckGrounded()
    {

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            if (state == State.Ragdoll)
                state = State.Normal;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
        }

        return m_IsGrounded;
    }

    float CurrentJobPiority = 1;
    void Update()
    {
        //rigidbody.velocity = Vector3.up+Vector3.forward;
        CheckGrounded();



		if (state == State.Normal)
		{

			if (ActiveBehavior == null)
				SwitchBehavior();
			else if (ActiveBehavior.SwitchToAny)
			{
				var dict = Behaviors
					.Where(x => x.ComeFromAny)
					.ToDictionary(x => x, b => b.CurrentPriority)
					.Where(x => x.Value > CurrentJobPiority);

				if (dict.Any())
				{
					var next = dict.Where(x => x.Key != ActiveBehavior).MaxBy(x => x.Value).Key;
					//Debug.Log($"overide behvior to {next.GetType().Name}");
					SwitchBehavior(next);
				}
			}

			//Behaviors.Where()

			ActiveBehavior?.Run();

		}
        UpdateAnimator();

        //Debug.DrawLine(agent.steeringTarget, this.pos());
        //Debug.DrawRay(transform.pos(), agent.desiredVelocity, Color.cyan);
    }


    public void Fling(Vector3 velocity)
    {
		if (!rigidbody || float.IsNaN(velocity.magnitude))
			return;

        state = State.Ragdoll;
        rigidbody.velocity = velocity;
    }


    /// <summary>
    /// act if i was thrown
    /// </summary>
    /// <param name="velocity">How fast and hard</param>
    [Obsolete]
    internal void Throw(Vector3 velocity)
    {
        //SwitchBehavior<FallingBehavior>();
        Fling(velocity);
        //throw new NotImplementedException();
    }


	private void FixedUpdate()
	{
		UpdateMovment();
	}
	void UpdateAnimator()
    {

        var vel = transform.InverseTransformVector(rigidbody.velocity);

        var yaw = transform.rotation.y;

        var turn = (yaw - lastYaw) / Time.deltaTime;
        lastYaw = yaw;

		Animator.SetBool("OnGround", IsGrounded);
		Animator.SetFloat("Forward", vel.z, 0.1f, Time.deltaTime);
        Animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);

    }


    public void SetTarget(UnityEngine.Transform target, float distance)
    {
        targetDistanceGoal = distance;
        SetTarget(target);
    }



	public bool navDirty = false;
    public void SetTarget(UnityEngine.Transform target)
    {
        this.target = target;
		navDirty = true;
		navigationActive = true;
		//if (agent.isActiveAndEnabled)
		//agent.isStopped = false;
	}


    public void SetTarget(Vector3 pos)
    {
		navDirty = true;
		navigationActive = true;
		//agent.isStopped = false;
		target = null;
        targetpos = pos;
    }
    // Update is called once per frame
    void UpdateMovment()
    {
		if (!navigationActive)
			return;

        //agent.updatePosition = character.IsGrounded;
        if (target)
            targetpos = target.position;
        else
            target = null;
		
        if (agent && agent.isOnNavMesh)
            if (navDirty || Vector3.Distance(lastTargetPos, targetpos) > targetPosMoveThreshold)
            {
				navDirty = false;
                agent.SetDestination(Vector3Int.RoundToInt(targetpos));
                lastTargetPos = targetpos;
            }

        var speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
        speed = agent.desiredVelocity.magnitude;
        var realVel = speed * transform.forward;
        //realVel = transform.InverseTransformDirection(agent.desiredVelocity).z*transform.forward;
        realVel = agent.desiredVelocity.magnitude * (agent.steeringTarget - this.pos()).normalized;

        var lookDir = realVel;

        lookDir += 0.1f * (targetpos - this.pos()).normalized;
        lookDir = lookDir.x0z();
		if (lookDir.magnitude > float.Epsilon)
		{
			var rotation = Quaternion.LookRotation(lookDir);
			//transform.rotation = rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
		}
        //if (agent.remainingDistance > agent.stoppingDistance)
        Move(realVel);
        //else mob.Move(Vector3.zero);
    }

    public float rotationSpeed = 3;
    public UnityEngine.Transform head;

    public bool AtTarget => Vector3.Distance(transform.position, targetpos) < targetDistanceGoal;


	bool navigationActive = false;
	public IJob job;

	public void TargetClear()
    {

        target = null;
		navigationActive = false;
		Move(Vector3.zero);
		//if (!agent || !agent.isOnNavMesh) return;
		//agent.isStopped = true;
	}

    public override bool Equals(object obj)
    {
        var mob = obj as Mob;
        return mob != null &&
               base.Equals(obj) && mob.gameObject == gameObject;
    }

    public override int GetHashCode()
    {
        return gameObject.GetHashCode();
    }
}
