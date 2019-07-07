using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storeroom : MonoBehaviour
{
	public CooldownTimer timer = new CooldownTimer(10);

	public float maxBalance = 50;
	public Team team;
	public float robPenalty = 1;
	public void Rob(Team by)
	{
		if (by == team || !timer.Check)
			return;

		
		var stolen = Mathf.Min(maxBalance, (float)team.Balance);
		team.AddRep(by, -robPenalty * stolen);
		team.Balance -= stolen;
		by.Balance += stolen;
		
	}
}
