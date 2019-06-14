using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class Mob : MonoBehaviour
{
    Rigidbody carrying;

	NavMeshAgent agent;
	Animator animator;

    public AIMovement Movement { get; private set; }

    new Rigidbody rigidbody;
	public Health health { get; private set; }

    public AiBehavior[] Behaviors => new AiBehavior[0];

    public float maxSpeed = 1;

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
        animator.applyRootMotion = false;
		agent = GetComponent<NavMeshAgent>();
        Movement = GetComponent<AIMovement>();
        if (agent)
        {
            var mode = false;
            agent.updatePosition = false;
            agent.updateUpAxis = false;
            agent.updateRotation = true;
        }
		health = GetComponent<Health>();
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;   
    }


    public void Move(Vector3 targetVel, float turn=0)
    {
        //targetVel = transform.TransformVector(targetVel);
        if (IsGrounded)
        {
            targetVel.y = rigidbody.velocity.y;
            rigidbody.velocity = targetVel;
            //rigidbody.angularVelocity = Vector3.up * turn;
        }
    }


    
	public Team Team => health.team;

    private bool m_IsGrounded;
    private Vector3 m_GroundNormal;
    public float GroundCheckDistance = 1f;
    public bool IsGrounded { get => m_IsGrounded; internal set { m_IsGrounded = false; } }


    bool CheckGrounded() {

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
        }

        return m_IsGrounded;
    }

    void Update()
    {
        //rigidbody.velocity = Vector3.up+Vector3.forward;
        CheckGrounded();
        agent.enabled = m_IsGrounded;
        agent.updatePosition = false && m_IsGrounded;

        UpdateAnimator();
        Debug.DrawLine(agent.steeringTarget, this.pos());
        Debug.DrawRay(transform.pos(), agent.desiredVelocity, Color.cyan);
    }



    float lastYaw;

    void UpdateAnimator()
    {
        animator.SetBool("OnGround", IsGrounded);

        var vel = transform.InverseTransformVector(rigidbody.velocity);

        var yaw = transform.rotation.y;

        var turn = (yaw - lastYaw) / Time.deltaTime;
        lastYaw = yaw;

        animator.SetFloat("Forward", vel.z, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);

    }


    public Vector3 targetpos;
    public float targetDistace;
    
    public Transform target;                                    // target to aim for

    public void SetTarget(Transform target)
    {
        this.target = target;
        if (agent.isActiveAndEnabled)
            agent.isStopped = false;
    }


    public float targetDistanceGoal = 1;

    Vector3 lastTargetPos;
    public float targetPosMoveThreshold = 1;


    public void SetTarget(Vector3 pos)
    {
        agent.SetDestination(pos);
        agent.isStopped = false;
        target = null;
        targetpos = pos;
    }
    // Update is called once per frame
    void UpdateMovment()
    {
        //agent.updatePosition = character.IsGrounded;
        if (target)
            targetpos = target.position;
        else
            target = null;

        if (agent.isActiveAndEnabled)
            if (Vector3.Distance(lastTargetPos, targetpos) > targetPosMoveThreshold)
            {

                agent.SetDestination(targetpos);
                lastTargetPos = targetpos;
            }

        var speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
        speed = agent.desiredVelocity.magnitude;
        var realVel = speed * transform.forward;
        //realVel = transform.InverseTransformDirection(agent.desiredVelocity).z*transform.forward;
        realVel = agent.desiredVelocity.magnitude * (agent.steeringTarget - this.pos()).normalized;
        //if (agent.remainingDistance > agent.stoppingDistance)
        mob.Move(realVel);
        //else mob.Move(Vector3.zero);
    }



    public bool AtTarget => Vector3.Distance(transform.position, targetpos) < targetDistanceGoal;

    public void Clear()
    {
        target = null;
        if (!agent || !agent.isOnNavMesh) return;
        agent.isStopped = true;
    }
}
