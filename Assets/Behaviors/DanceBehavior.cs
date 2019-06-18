using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceBehavior : AiBehavior
{

    public float danceDuration = 10;

    public override bool OnBegin()
    {
        Me.Animator.SetBool("Dance", true);
        return base.OnBegin();
    }
    public override bool OnEnd()
    {
        Me.Animator.SetBool("Dance", false);
        return base.OnBegin();
    }

    public override void Run()
    {
        if(Time.time - startTime > danceDuration)
        {
            Me.SwitchBehavior();
        }
        base.Run();
    }
}
