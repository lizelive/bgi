using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class FollowBehavior : AiBehavior
{
    public override bool SwitchToAny => true;
    public Player following;
	public float maxFolowRange = 10;
		
    public override bool OnBegin()
    {
        if(!following)
			following = FindObjectOfType<Player>();
        if (following && Me.Distance(following) < maxFolowRange)
        {
            Me.SetTarget(following.followPoint.transform);
            following.Followers.Add(Me);
            return base.OnBegin();
        }
        //print("Nothing to follow");
        return false;
	}

	public override bool OnEnd()
    {
        if(following)
        following.Followers.Remove(Me);

        Me.TargetClear();
        return base.OnEnd();
    }


	public void OnDestroy()
	{
		if(following)
			following.Followers.Remove(Me);
	}
	public override void Run()
    {
		if (!following)
			End();
		//Me.SwitchBehavior<PanicBehavior>();
		else if (Me.Distance(following) > maxFolowRange)
		{
			following = null;
			End();
		}
		base.Run();
    }
}
