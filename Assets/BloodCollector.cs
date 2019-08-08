using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodCollector : MonoBehaviour
{
    public Team Team;
    
    public float range = 20;
    public float multiplier = 3;
    // Start is called before the first frame update
    void HandleBlood(Health bleading, float hurt)
    {
        Team.Balance += multiplier * hurt;
    }
    
}
