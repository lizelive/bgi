using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Team : MonoBehaviour
{
	public double Balance = 0;

	public readonly Team Gia = null;

	public Color color;


	public bool autoAlly;
	public float fightThreshold = -3;

	public HashSet<Mob> mobs = new HashSet<Mob>();
	public HashSet<Health> members = new HashSet<Health>();

	public Dictionary<Team, float> reputations = new Dictionary<Team, float>();


	public IEnumerable<T> GetMembers<T>()
	{
		foreach (var member in members)
		{
			var mcguffin = member.GetComponent<T>();
			if (mcguffin != null) yield return mcguffin;
		}
	}


	public float TotalRep => reputations.Sum(u => u.Value);

	public float Confidance => Mathf.Max(0, TotalRep / GetRep(this));

	public float GetRep(Team team)
	{
		var val = reputations.TryGetValue(team, out var value) ? value : 0;
		if (float.IsNaN(val))
			val = 0;
		return val;
	}

	public float SetRep(Team team, float value)
	{
		return reputations[team] = value;
	}

	public float AddRep(Team team, float value)
	{
		print($"{name} gave {team} {value} rep");
		return SetRep(team, GetRep(team) + value);
	}


	public List<Team> Allies;

	// Start is called before the first frame update
	void Start()
	{

	}

	public bool Fighting(Team other)
	{
		return Fighting(this, other);
	}

	public bool Fighting(Health other)
	{
		return Fighting(this, other.team);
	}

	public static bool Fighting(Team a, Team b)
	{
		if (a == null || b == null)
			return true;
		if (a == b) return false;

		if (a.autoAlly)
		{
			return (a.GetRep(b) <= a.fightThreshold);
		}
		else
		{
			return !a.Allies.Contains(b);
		}
	}

	internal bool Unregister(Health health)
	{
		var yay = members.Remove(health);
		var mob = health.GetComponent<Mob>();
		if (mob)
			mobs.Remove(mob);
		return yay;
	}

	internal void Register(Health health)
	{
		members.Remove(health);
		var mob = health.GetComponent<Mob>();
		if (mob)
			mobs.Remove(mob);
	}
}
