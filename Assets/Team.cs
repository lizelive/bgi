﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{

    public readonly Team Gia = null;

    public Color color;

    public List<Norb> norbs;
    public List<Player> players;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool Fighting(Team other)
    {
        return Fighting(this, other);
    }

    public static bool Fighting(Team a, Team b)
    {
        return a != b;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
