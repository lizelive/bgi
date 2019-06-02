using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(ThirdPersonCharacter))]
public class AIMovement : MonoBehaviour
{

    public Vector3 targetpos;
    public float targetDistace;


    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public ThirdPersonCharacter character { get; private set; } // the character we are controlling
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
        agent.updatePosition = character.IsGrounded;
        if (target)
            targetpos = target.position;
        else
            target = null;

        if (Vector3.Distance(lastTargetPos, targetpos) > targetPosMoveThreshold)
        {

            agent.SetDestination(targetpos);
            lastTargetPos = targetpos;
        }
        

        if (agent.remainingDistance > agent.stoppingDistance)
            character.Move(agent.desiredVelocity, false, false);
        else
            character.Move(Vector3.zero, false, false);
    }

    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();

        agent.updateRotation = false;
        agent.updatePosition = false;
    }

    public bool AtTarget => Vector3.Distance(transform.position, targetpos) < targetDistanceGoal;
}
