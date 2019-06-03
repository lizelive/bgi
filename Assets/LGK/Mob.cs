using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]

public class Mob : MonoBehaviour
{
	NavMeshAgent agent;
	Animator animator;
    new Rigidbody rigidbody;
	public Health health { get; private set; }

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
        if (agent)
        {
            var mode = false;
            agent.updatePosition = false;
            agent.updateUpAxis = true;
            agent.updateRotation = true;
        }
		health = GetComponent<Health>();
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }


    public void Move(Vector3 targetVel, float turn=0)
    {

        if (IsGrounded)
        {
            print("TV" + targetVel);
            //targetVel.y = rigidbody.velocity.y;
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
        UpdateAnimator();
        Debug.DrawLine(agent.steeringTarget, this.pos());
        Debug.DrawRay(transform.pos(), agent.desiredVelocity, Color.cyan);
    }


    void UpdateAnimator()
    {
        animator.SetBool("OnGround", IsGrounded);

        var vel = transform.InverseTransformVector(rigidbody.velocity);

        animator.SetFloat("Forward", vel.z, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", vel.x, 0.1f, Time.deltaTime);

    }


}
