using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceBehavior : AiBehavior
{
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
}
