using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class FollowBehavior : AiBehavior
{
    public override bool SwitchToAny => true;
    public Player following;

    public override bool OnBegin()
    {
        if(!following)
        following = FindObjectOfType<Player>();
        if (following)
        {
            Me.SetTarget(following.followPoint.transform);
            following.Followers.Add(Me);
            return base.OnBegin();
        }
        print("Nothing to follow");
        return false;
    }

    public override bool OnEnd()
    {
        if(following)
        following.Followers.Remove(Me);

        Me.TargetClear();
        return base.OnEnd();
    }

    public override void Run()
    {
        if (!following)
            Me.SwitchBehavior<PanicBehavior>();
        base.Run();
    }
}
