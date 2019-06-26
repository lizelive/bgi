using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class SmellFlowersBehavior : SeekBehavior<Flowery>
{
	public override bool SwitchToAny => true;
	public override void Interact(Flowery thing) {
        if (Smells >= thing.smells)
        {
            Me.SwitchBehavior();
            return;
        }
        if (Smells == 0)
        {
            Me.Animator.SetTrigger("SmellFlowers");
        }

        thing.lastSmelt = Time.time;
        Smells += Time.deltaTime;
    }
    public override void LockOn(Flowery thing)
    {
        Smells = 0;
        thing.lastSmelt = Time.time;
        base.LockOn(thing);
    }

    public override bool IsValid(Flowery thing)
    {
        return thing.IsValid;
    }
    
    public float Smells;
}
