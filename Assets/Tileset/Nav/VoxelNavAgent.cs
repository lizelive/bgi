using UnityEngine;

public class VoxelNavAgent : MonoBehaviour
{
    public float speed = 6;
    public float turnSpeed = 6;

    public Vector3 desiredVelocity;

    Pathplanner pathplanner = new Pathplanner();


    NavPath path;
    int pathIndex = 0;

    Transform targetObject;
    public Vector3Int steeringTarget;

    Vector3Int lastTagetPos;

    public void SetDestination(Transform target)
    {
        targetObject = target;
    }

    public void SetDestination(Vector3Int target)
    {
        targetObject = null;
        steeringTarget = target;
    }
    // Start is called before the first frame update
    void Awake()
    {

        pathplanner.world = FindObjectOfType<VoxelWorld>();
    }

    int timeOnStep = 0;

    bool navDirty = false;
    public bool updatePosition;

    public bool isOnNavMesh => true;


    private void OnDrawGizmosSelected()
    {
        if (path != null)
        {
            Vector3 last = transform.pos();
            foreach (var step in path.steps)
            {
                Debug.DrawLine(last, step);
                last = step;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (targetObject)
        {
            steeringTarget = Vector3Int.RoundToInt(targetObject.pos());
        }

        if (lastTagetPos != steeringTarget)
        {
            navDirty = true;
        }

        lastTagetPos = steeringTarget;

        var curPos = Vector3Int.RoundToInt(transform.pos() + 0.5f * Vector3.up);
        if (navDirty)
        {
            path = pathplanner.AStar(curPos, steeringTarget);
            navDirty = path == null;
            pathIndex = 0;
            timeOnStep = 0;

        }

        if (path!=null && pathIndex >= path.steps.Length)
        {
            path = null;
            // i am at my goal
        }

        if (path != null)
        {
            var goalPos = path.steps[pathIndex];

            var distToNextGoal = Vector3Int.Distance(goalPos, curPos);

            // check if got too far from goal
            if (distToNextGoal > 5 || timeOnStep > 100)
            {
                navDirty = true;
            }

            // check if stuck

            if (curPos == goalPos)
            {
                pathIndex++;
                timeOnStep = 0;
            }
            else
            {
                timeOnStep++;
            }

            desiredVelocity = ((Vector3)(goalPos - curPos)).normalized * speed;
        }
        if (updatePosition)
        {
            transform.position += desiredVelocity * Time.deltaTime;
        }
    }
}
