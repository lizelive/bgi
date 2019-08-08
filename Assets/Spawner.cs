using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public Team team;
	public GameObject spawns;
	public int numToSpawn = 1;
	public float lastSpawned = float.NegativeInfinity;
	public float spawnInterval = 1;
	public float spawnCost = 0; 
	public List<GameObject> spawned;
	public UnityEngine.Transform spawnPoint;

	//public event Func<Spawner,bool> ShouldSpawn;

	public event Action<GameObject> Spawned;
	private void Start()
	{
		if (!spawnPoint)
			spawnPoint = transform;
	}
	// Update is called once per frame
	void Update()
    {
		var now = Time.time;
		spawned.RemoveAll(x => !x);
		if (now - lastSpawned > spawnInterval && spawned.Count < numToSpawn && (!team || team.Balance >= spawnCost) )
		{
			var yum = Instantiate(spawns, spawnPoint.position, spawnPoint.rotation);
			spawned.Add(yum);
			Spawned?.Invoke(yum);
			lastSpawned = now;
		}
    }

}
