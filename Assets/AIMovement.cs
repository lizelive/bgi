using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(Mob))]
public class AIMovement : MonoBehaviour
{

    public Vector3 targetpos;
    public float targetDistace;
    Mob mob;

    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public Transform target;                                    // target to aim for

    public void SetTarget(Transform target)
    {
        this.target = target;
    }


    public float targetDistanceGoal = 1;

    Vector3 lastTargetPos;
    public float targetPosMoveThreshold = 1;


    public void SetTarget (Vector3 pos)
    {
        target = null;
        targetpos = pos;
    }
    // Update is called once per frame
    void Update()
    {
        //agent.updatePosition = character.IsGrounded;
        if (target)
            targetpos = target.position;
        else
            target = null;

        if (Vector3.Distance(lastTargetPos, targetpos) > targetPosMoveThreshold)
        {

            agent.SetDestination(targetpos);
            lastTargetPos = targetpos;
        }

        var speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
        speed = agent.desiredVelocity.magnitude;
        var realVel = speed * transform.forward;
        //realVel = transform.InverseTransformDirection(agent.desiredVelocity).z*transform.forward;

        //if (agent.remainingDistance > agent.stoppingDistance)
        mob.Move(agent.desiredVelocity);
        //else mob.Move(Vector3.zero);
    }

    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        mob = GetComponent<Mob>();
    }

    public bool AtTarget => Vector3.Distance(transform.position, targetpos) < targetDistanceGoal;
}
