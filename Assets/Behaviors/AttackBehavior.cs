using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class AttackBehavior : AiBehavior
{
    public Health target;
    public float MidAttackPriority = 1000;
    public override float CurrentPriority => target? MidAttackPriority : Me.NearbyEnemies.Any() ? BasePriority / Me.NearbyEnemies.Min(Me.Distance) : 0;
    // Start is called before the first frame update
    public override bool OnBegin()
    {
        if (!target)
        {
            target = Me.NearbyEnemies
				.OrderBy(Me.Distance)
                .FirstOrDefault(Me.CanSee)
				?.Health;
        }
        
        if (!target)
        {
            return false;
        }
        Me.SetTarget(target.transform);

        return base.OnBegin();
    }

    public override bool OnEnd()
    {
        target = null;
        return base.OnEnd();
    }

    public override void Run()
    {
        if (!target)
        {
            End();
            return;
        }
        base.Run();
        Me.SetTarget(target.transform, 2);

        if (Me.AtTarget)
        {
            GetComponent<MeleeWeapon>()?.Attack();
        }
    }
}
