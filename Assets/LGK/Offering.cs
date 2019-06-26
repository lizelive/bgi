using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Offering : MonoBehaviour
{
    public float maxOffering;
    public float curret;
    public float OfferingP => curret/ maxOffering;
    public int startI = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        var numOffering = OfferingP * transform.childCount;
        for (int i = startI; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i <= numOffering);
        }
    }
}
