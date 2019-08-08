using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// handles path planning dispatches
/// cashes prior results for improved performance..
/// </summary>
public class Pathplanner 
{
    public VoxelWorld world;

    public UnityEngine.Transform target;


    //private void Update()
    //{
    //    var start = Vector3Int.RoundToInt(target.pos()+ 0.5f*Vector3.up);
    //    var end = Vector3Int.RoundToInt(transform.pos());
    //    var path = AStar(start, end);
    //    if (path != null)
    //    {
    //        Vector3 last = transform.pos();
    //        foreach (var step in path.steps)
    //        {
    //            Debug.DrawLine(last, step);
    //            last = step;
    //        }
    //    }
    //    else
    //    {
    //        Debug.DrawLine(start, end, Color.blue);
    //    }
    //}

    class Step : System.IComparable<Step>
    {
        public float cost;
        public Vector3Int pos;
        public Step from;
        readonly public int stepNumber;
       

        public Step(Vector3Int to, Step from, float totalDistance)
        {
            this.pos = to;
            this.from = from;
            this.cost = totalDistance;
            stepNumber = (from?.stepNumber ?? 0) + 1;
        }

        public override bool Equals(object obj)
        {
            var step = obj as Step;
            return step != null &&
                   pos.Equals(step.pos) &&
                   cost == step.cost;
        }

        public override int GetHashCode()
        {
            var hashCode = -476541541;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3Int>.Default.GetHashCode(pos);
            hashCode = hashCode * -1521134295 + cost.GetHashCode();
            return hashCode;
        }

        public int CompareTo(Step other)
        {
            return cost.CompareTo(other?.cost);
        }
    }

    public NavPath AStar(Vector3Int start, Vector3Int end, int maxSteps = 128)
    {
        if (Vector3Int.Distance(start, end) > maxSteps)
            return null;
        var howIGo = new Dictionary<Vector3Int, Step>();


        var seenBefore = new HashSet<Vector3Int>();
        var openSpot = new PairingMinHeap<Step>();


        seenBefore.Add(start);

        float hueristic(Vector3Int p) => Vector3Int.Distance(p, end);
        openSpot.Add(new Step(start, null, hueristic(start)));

        while (openSpot.Count > 0)
        {
            var chain = openSpot.Extract();

            if (chain.pos == end)
            {
                var steps = new List<Vector3Int>();
                while (chain != null)
                {
                    steps.Add(chain.pos);
                    chain = chain.from;
                }
                steps.Reverse();
                return new NavPath(steps.ToArray());
            }

            if (chain.stepNumber < maxSteps)
                foreach (var move in ValidMoves(chain.pos).Where(x => !seenBefore.Contains(x)))
                {
                    seenBefore.Add(move);
                    var cost = chain.stepNumber + 1 + hueristic(move);
                    openSpot.Add(new Step(move, chain, cost));

                    Debug.DrawLine(chain.pos, move, Color.red);
                }
        }

        //var open = name
        return null;
    }

    public bool IsValidStandingSpot(Vector3Int pos, int agentHeight)
    {
        return world[pos + Vector3Int.down].IsSolid && Enumerable
            .Range(pos.y, agentHeight)
            .All(y =>
            {
                var p = pos;
                p.y = y;
                return !world[pos].IsSolid;
            });
    }

    public IEnumerable<Vector3Int> ValidMoves(
        Vector3Int pos,
        int agentHeight = 2,
        int maxJump = 1,
        int maxFall = 4)
    {
        // move like a normal person
        foreach (var dir in VecU.CardnalDirs)
        {
            var foundValidSpot = false;

            for (int dy = 0; dy <= maxJump; dy++)
            {
                var p = pos + dir;
                p.y += dy;
                if (IsValidStandingSpot(p, agentHeight))
                {
                    yield return p;
                    foundValidSpot = true;
                }
            }
            if (!foundValidSpot)
            {
                for (int dy = 0; dy <= maxFall; dy++)
                {
                    var p = pos + dir;
                    p.y -= dy;
                    if (IsValidStandingSpot(p, agentHeight))
                    {
                        yield return p;
                        foundValidSpot = true;
                    }
                }
            }
        }
    }

}
