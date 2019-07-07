using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownTimer
{
	public float spacing;
	public float last = float.NegativeInfinity;

	public CooldownTimer(float spacing = 0)
	{
		this.spacing = spacing;
	}

	// Start is called before the first frame update
	public void Restart()
	{
		last = Now;
	}

	public float Now => Time.time;
	public bool Ready => Now - last >= spacing;
	public bool Check
	{
		get
		{
			if (Ready)
			{
				Restart();
				return true;
			}
			return false;
		}
	}

}
