using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class RobBehavior : SeekBehavior<TreasureChest>
{
    public override float TargetDistance => 3;
    public override bool IsValid(TreasureChest thing)
    {
        return Me.Team.Fighting(thing.team) && thing.Value > 0;
    }

    public override void Interact(TreasureChest thing)
    {
        Team.Balance += thing.Value;
        thing.Value = 0;
        End();
    }
}
