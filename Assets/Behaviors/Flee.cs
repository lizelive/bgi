using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Flee : AiBehavior
{
    public float priority;

    public Transform[] scaryThings;
    public float MaxStepHight = 1;
    public float FleeRange = 5;


    public float GiveUpThreshold;
    // Start is called before the first frame update
    public void Run()
    {
        
    }

    public int numberOfTries = 10;


    public float EvalSpot(Vector3 pos)
    {
        var sumOfSquares = scaryThings.Sum(boo => 1/boo.Distance(pos));
        return sumOfSquares;
    }

    public IEnumerable<Vector3> PossibleLocations()
    {
        for (int i = 0; i < numberOfTries; i++)
        {
            var startPos = FleeRange * Random.insideUnitCircle.normalized.x0y() + this.pos() + MaxStepHight * Vector3.up;

            var direction = Vector3.down;
            if (Physics.Raycast(startPos, direction, out var hit))
            {
                yield return hit.point;
            }
        }
    }

    public Vector3 FindNewHidingSpot()
    {
        var newTarget = PossibleLocations().MinBy(EvalSpot);
        Debug.DrawLine(transform.position, newTarget, Color.yellow);
        return newTarget;
    }

    private void Update()
    {
        FindNewHidingSpot();
    }

}
