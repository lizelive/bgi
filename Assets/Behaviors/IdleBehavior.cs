using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : AiBehavior
{
	public override bool SwitchToAny => true;
	public override void Run()
    {
		Me.Move(Vector3.zero);
		End();
        //print($"I tried an i {Me.SwitchBehavior()}");
    }
}
