using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storeroom : MonoBehaviour
{
	public float maxBalance = 50;
	public Team team;
	public float robPenalty = 1;
	public void Rob(Team by)
	{
		if (by == team)
			return;

		team.AddRep(by, -robPenalty);
		var stolen = Mathf.Min(maxBalance, team.Balance);
		team.Balance -= stolen;
		by.Balance += stolen;
		
	}
}
