using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IJob
{
    Type TypeOfBehavior { get; }
    Team[] ValidTeams { get; }
    bool IsMobWorth(Mob mob);
}

public class JobManager : MonoBehaviour
{


    public void PublishJob(IJob job)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
