using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class Mob : MonoBehaviour
{
    public AiBehavior[] Behaviors = new AiBehavior[0];


    public AiBehavior ActiveBehavior;

    public bool SwitchBehavior(AiBehavior next)
    {
        if (ActiveBehavior)
        {
            if (!ActiveBehavior.OnEnd())
            {
                return false;
            }
            else
                ActiveBehavior = null;
        }
        if (next?.OnBegin() ?? true)
        {
            ActiveBehavior = next;

            return true;
        }
        else return false;

    }

    public bool SwitchBehavior()
    {
        return SwitchBehavior(Behaviors.MaxBy(b => b.CurrentPriority));
    }


    public bool SwitchBehavior<T>() where T : AiBehavior
    {
        var next = GetComponent<T>();
        if (!next)
        {
            Debug.LogError($"Missing behavior {typeof(T).Name}");
        }
        return SwitchBehavior(next);

    }



    Rigidbody carrying;

    NavMeshAgent agent;
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
    public Team Team => Health.team;

    private bool m_IsGrounded;
    private Vector3 m_GroundNormal;
    public float GroundCheckDistance = 1f;
    public bool IsGrounded { get => m_IsGrounded; internal set { m_IsGrounded = false; } }
    public Vector3 targetpos;
    public float targetDistace;

    public Transform target;                                    // target to aim for

    public float targetDistanceGoal = 1;

    Vector3 lastTargetPos;
    public float targetPosMoveThreshold = 1;

    float lastYaw;



    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        Animator.applyRootMotion = false;
        agent = GetComponent<NavMeshAgent>();

        Behaviors = GetComponents<AiBehavior>();

        if (agent)
        {
            var mode = false;
            agent.updatePosition = false;
            agent.updateUpAxis = false;
            agent.updateRotation = false;
        }
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        SwitchBehavior();
    }


    public void Move(Vector3 targetVel, float turn = 0)
    {
        //targetVel = transform.TransformVector(targetVel);
        if (IsGrounded)
        {
            targetVel.y = rigidbody.velocity.y;
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
        agent.enabled = m_IsGrounded;
        agent.updatePosition = false && m_IsGrounded;


        var dict = Behaviors.Where(x => x.ComeFromAny).ToDictionary(x => x, b => b.CurrentPriority).Where(x => x.Value > CurrentJobPiority);

        if (dict.Any())
        {
            var next = dict.MaxBy(x => x.Value).Key;
            SwitchBehavior(next);
        }
        if (ActiveBehavior == null)
            SwitchBehavior();
        ActiveBehavior?.Run();

        UpdateAnimator();
        UpdateMovment();

        Debug.DrawLine(agent.steeringTarget, this.pos());
        Debug.DrawRay(transform.pos(), agent.desiredVelocity, Color.cyan);
    }




    void UpdateAnimator()
    {
        Animator.SetBool("OnGround", IsGrounded);

        var vel = transform.InverseTransformVector(rigidbody.velocity);

        var yaw = transform.rotation.y;

        var turn = (yaw - lastYaw) / Time.deltaTime;
        lastYaw = yaw;

        Animator.SetFloat("Forward", vel.z, 0.1f, Time.deltaTime);
        Animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);

    }


    public void SetTarget(Transform target, float distance)
    {
        targetDistanceGoal = distance;
        SetTarget(target);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        if (agent.isActiveAndEnabled)
            agent.isStopped = false;
    }


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

        var lookDir = realVel.x0z();
        var rotation = Quaternion.LookRotation(lookDir);
        transform.rotation = rotation;
        //if (agent.remainingDistance > agent.stoppingDistance)
        Move(realVel);
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
