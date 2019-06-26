using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public float Balance;

    public readonly Team Gia = null;

    public Color color;

    public List<Norb> norbs;
    public List<Player> players;



	Dictionary<Team, float> reputation = new Dictionary<Team, float>();



	public float GetRep(Team team)
	{
		float value = 0;
		reputation.TryGetValue(team, out value);
		return value;
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
        return a != b;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
