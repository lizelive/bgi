using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public float growthRate= 1 / 60f;
    public GameObject cropType;

    public float growth;

    public float harvestPerSecond = 1;
    public float maxGrowth = 1;

    public bool Ready => growth == maxGrowth;

    public float PGrowth => growth / maxGrowth;

    // Start is called before the first frame update
    void Start()
    {

    }
    
    public float Harvest(float amount)
    {
        var harvestAmmount = Mathf.Min(amount, growth);
        growth -= harvestAmmount;
        return harvestAmmount;
    }

    // Update is called once per frame
    void Update()
    {
        growth = Mathf.Min(growth + growthRate * Time.deltaTime, maxGrowth);

        var growthP = growth / maxGrowth;
        var numApples= growthP*transform.childCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i <= numApples);
        }

    }
}
