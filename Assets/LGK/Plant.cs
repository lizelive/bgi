using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

	public Team Owner => GetComponent<Health>()?.team;

    public float growthRate= 1 / 60f;
    public GameObject cropType;

    public float growth;

    public float harvestPerSecond = 1;
    public float maxGrowth = 1;

	public bool Ready => growth == maxGrowth && Time.time - lastLookat > 1;
	public float lastLookat;

	public float PGrowth => growth / maxGrowth;
    
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
		var numApples = growthP * (transform.childCount - 1);

        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i <= numApples);
        }

    }
}
