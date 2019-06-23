using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class SeekBehavior<T> : AiBehavior where T: UnityEngine.Component
{
    public T target;

    public override float CurrentPriority => 
        gameObject.Find<T>(Me.ViewRange).Where(IsValid).Any() ? BasePriority : 0;

    public virtual float TargetDistance => 1;

    public virtual bool IsValid(T thing) => thing;
    public virtual void LockOn(T thing) { }
    public virtual void Interact(T thing) { }
    public override bool OnBegin()
    {

        if (!target)
            target = gameObject.Find<T>(Me.ViewRange).Where(IsValid).Random();


        if (target)
        {
            LockOn(target);
            Me.SetTarget(target.transform,TargetDistance);
            return base.OnBegin();
        }
        return false;
    }
    public override bool OnEnd()
    {
        target = null;
        return base.OnEnd();
    }
    public override void Run()
    {
        Me.SetTarget(target.transform, TargetDistance);
        if (Me.AtTarget)
        {
            Interact(target);
        }
    }
}
