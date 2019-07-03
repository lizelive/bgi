using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class DepositBehavior : SeekBehavior<Offering>
{
    public override float TargetDistance => 4;
    public override float CurrentPriority => 1;

    public override void Interact(Offering thing)
    {
        //print("EA get in the game");
        Me.SwitchBehavior();
    }
}
