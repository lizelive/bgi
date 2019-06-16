using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : AiBehavior
{
    public override void Run()
    {
        print($"I tried an i {Me.SwitchBehavior()}");
    }
}
