using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    public float growTime;
    public Egg thingToSpawn;
    public UnityEngine.Transform spawnPoint;
    float last;
    public Team owner;

    Egg lastSpawned;

    public float farEnough = 3;

    // Start is called before the first frame update
    void Start()
    {
        last = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastSpawned!=null)
            if (!lastSpawned || gameObject.Distance(lastSpawned) > farEnough)
        {
            last = Time.time;
            lastSpawned = null;
        }
        if (!lastSpawned && growTime < Time.time - last)
        {
            last = Time.time;
            lastSpawned = Instantiate(thingToSpawn, spawnPoint.position, spawnPoint.rotation);
            lastSpawned.owner = owner;
        }
    }
}
