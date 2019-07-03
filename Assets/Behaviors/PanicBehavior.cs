using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicBehavior : AiBehavior
{

    public override bool OnBegin()
    {
        Me.Animator.SetBool("Panic", true);
        Me.TargetClear();
		
        return base.OnBegin();
    }
    public override bool OnEnd()
    {
        Me.Animator.SetBool("Panic", false);
        return base.OnBegin();
    }


    public override void Run()
    {
		Me.Move(Vector3.zero);
		//print("Reeeeeeeeeeeeee");
	}
}
