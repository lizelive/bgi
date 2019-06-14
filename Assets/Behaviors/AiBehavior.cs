using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AiBehavior : MonoBehaviour
{

    public static AiBehavior Idle => new AiBehavior();


    // need to figure out how these work
    public Mob me;
    public Team team;


    protected IEnumerable<Mob> Nearby;
    protected IEnumerable<Mob> NearbyEnemy;
    IEnumerable<AiBehavior> AllPossible;
    public float BasePriority;
    protected float ViewRange;


    public virtual bool ComeFromIdle => false;
    public virtual bool ComeFromAny => false;

    public IEnumerable<AiBehavior> GetAllPossibleFrom()
    {
        if (ComeFromIdle)
            yield return Idle;
        if(ComeFromAny)
            foreach (var item in AllPossible)
            {
                yield return item;
            }
    }

    public float CurrentPriority() => 0;


    void Start()
    {
        me = GetComponent<Mob>();
    }


    /// <summary>
    /// This is not at weird at all. Do not question it. Needed for hash table and stuff
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        var other = obj?.GetType();
        var me = GetType();
        return other == me;
    }

    public override int GetHashCode()
    {
        return GetType().GetHashCode();
    }
}
