using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Team : MonoBehaviour
{
    public float Balance = 0;

    public readonly Team Gia = null;

    public Color color;

    public HashSet<Mob> mobs = new HashSet<Mob>();

	public Dictionary<Team, float> reputation = new Dictionary<Team, float>();



	public float TotalRep => reputation.Sum(u=>u.Value);

	public float Confidance => Mathf.Max(0,TotalRep/GetRep(this));

	public float GetRep(Team team)
	{
		return reputation.TryGetValue(team, out var value) ? value : 0;
	}

	public float SetRep(Team team,float value)
	{
		return reputation[team] = value;
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
		return !a.Allies.Contains(b);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
