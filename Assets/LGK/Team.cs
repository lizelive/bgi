using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(menuName ="BGI/Team")]
public class Team : ScriptableObject
{
	public int maxNumMobs = 100;
	public double balance = 0;

	public readonly Team Gia = null;

	public Color color;


	public int NumMobs => mobs.Count;

	public bool autoAlly;
	public float fightThreshold = -3;

	public List<Mob> mobs = new List<Mob>();
	public List<Health> members = new List<Health>();

	public Dictionary<Team, float> reputations = new Dictionary<Team, float>();
	public Dictionary<Team, float> loyaly = new Dictionary<Team, float>();

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

	public double Balance {
		get => balance;

		set {
			if (double.IsNaN(value))
				return;
			balance = value;
		}
	}

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
		//Debug.print($"{name} gave {team} {value} rep");
		return SetRep(team, GetRep(team) + value);
	}


	public List<Team> Allies;
	public JobManager JobManager;

	public float TotalHp => mobs.Sum(m => m.Health.CurrentHealth);

	public void ReportMurder(Team by, float damage)
	{
		AddRep(by, -damage/TotalHp);
		recentBloodshed += damage;
	}

	public bool Fighting(Mob other)
	{
		return Fighting(this, other.Team);
	}

	public bool Fighting(Team other)
	{
		return Fighting(this, other);
	}

	public bool Fighting(Health other)
	{
		return Fighting(this, other.team);
	}

	public float recentBloodshed = 0;
	public float bloodForgetRate = 1 / 360;
	public float lastTime;

	public float Fear
	{
		get
		{
			var now = Time.time;
			var deltat = now - lastTime;
			var numMobs = mobs.Count;
			var totalHp = TotalHp;
			recentBloodshed *= Mathf.Clamp01(1 - bloodForgetRate * deltat);
			lastTime = now;

			return recentBloodshed / numMobs;
		}
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
		members.Add(health);
		var mob = health.GetComponent<Mob>();
		if (mob)
			mobs.Add(mob);
	}


	public float lowFearThreshod = .2f;
	public float highFearThreshod = .8f;
	public float highRepThreshold = 0.5f;

	public enum Behavior
	{
		Normal,
		Curse,
		Fight,
		Summon,
		Praise,
		Pleed,
		Join
	}

	public Behavior WhatBehavior(Team to)
	{
		var rep = GetRep(to);
		var fear = Fear;



		var highFear = fear > highRepThreshold;
		var lowFear = fear < lowFearThreshod;
		var medFear = !(highFear || lowFear);


		var hasGoodRep = rep > highRepThreshold;
		var hasBadRep = rep < -highRepThreshold;
		var hasNutralRep = !(hasGoodRep || hasBadRep);


		if (hasBadRep)
		{
			if (highFear)
				return Behavior.Summon;
			if (medFear)
				return Behavior.Fight;
			if (lowFear)
				return Behavior.Curse;

		}

		if (hasNutralRep)
		{
			if (highFear)
				return Behavior.Pleed;
			if (medFear)
				return Behavior.Normal;
			if (lowFear)
				return Behavior.Normal;
		}

		if (hasGoodRep)
		{
			if (highFear)
				return Behavior.Join;
			if (medFear)
				return Behavior.Praise;
			if (lowFear)
				return Behavior.Praise;
		}

		throw new Exception("Invalid behavior state i guess. this should be imposible.");
		//return BehaviorStates.Ignore;
	}


}
