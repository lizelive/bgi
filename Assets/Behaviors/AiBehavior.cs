using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AiBehavior : MonoBehaviour
{

    public static AiBehavior Idle => new AiBehavior();


    // need to figure out how these work
    public Mob Me => GetComponent<Mob>();
    public Team Team => Me.Team;


    protected IEnumerable<Mob> Nearby;
    protected IEnumerable<Mob> NearbyEnemy;
    IEnumerable<AiBehavior> AllPossible;
    public float BasePriority;
    protected float ViewRange;


    public virtual bool ComeFromIdle => false;
    public virtual bool ComeFromAny => false;

    public float startTime;


    public virtual bool OnEnd() { return true; }
    public virtual bool OnBegin() {
        startTime = Time.time;
        print($"Begin {GetType().Name}");
        return true; }
    public virtual void Run() { }

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

    public virtual float CurrentPriority => BasePriority;




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
