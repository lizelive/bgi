using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FleeBehavior : AiBehavior
{
    Transform[] scaryThings = { };
    public float MaxStepHight = 1;
    public float FleeRange = 5;
    public float FeelSafeThreshold = 0.2f;
    public float PanicThreshold = 1;
    public int numberOfTries = 10;

	
    public float EvalSpot(Vector3 pos)
    {
        if (scaryThings ==null)
        {
            Debug.LogError("scarry stuff is null that is bad.");
            return 0;
        }
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

    static readonly bool DebugDraw = false;
    public override bool ComeFromAny => true;
    public override bool ComeFromIdle => true;

    public override float CurrentPriority
    {
        get
        {
            var currentFear = EvalSpot(this.pos());
            if (currentFear > FeelSafeThreshold)
            {
                return BasePriority;
            }
            return 0;
        }
    }

    public void Update()
    {
        scaryThings = Me.NearbyEnemies.Select(x=>x.transform).ToArray();
		if (!Running && CurrentPriority > 1)
			Me.SwitchBehavior(this);
	}

    public override void Run()
    {
        var currentFear = EvalSpot(this.pos());

        if (currentFear > PanicThreshold)
        {
            Me.SwitchBehavior<PanicBehavior>();
            return;

        }

        if(currentFear < FeelSafeThreshold)
        {
            Me.SwitchBehavior();
            return;
        }

        

        var newTarget = PossibleLocations().MinBy(EvalSpot);
        if (DebugDraw)
            Debug.DrawLine(transform.position, newTarget, Color.yellow);

        Me.SetTarget(newTarget);
    }

}
