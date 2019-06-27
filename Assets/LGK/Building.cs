using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
	public bool dropBuild;
	Player builder;
	public float buildSpawnHeight = 10;
	public void Place(Player by)
	{
		builder = by;
		if(dropBuild)
		Beckons();
	}

	private new Rigidbody  rigidbody;
	private void Beckons()
	{
		gameObject.isStatic = false;
		var rigidbody = GetComponent<Rigidbody>();

		var hadRb = !!rigidbody;
		if (!rigidbody)
		{
			rigidbody= gameObject.AddComponent<Rigidbody>();
		}

		var spawnHeight = buildSpawnHeight;


		transform.position += Vector3.up * spawnHeight;


		rigidbody.isKinematic = false;
		//rigidbody.constraints |= RigidbodyConstraints.FreezeRotation;




		//if(Physics.ray)


	}

	public void OnCollisionEnter(Collision collision)
	{
		//print($"buidling hit {collision.transform.gameObject}");
		//if (collision.transform.gameObject.isStatic)
		if (dropBuild && collision.transform.gameObject.isStatic)
		{
			gameObject.isStatic = true;
//			print($"buidling frezes");
			if(!rigidbody)
			rigidbody = GetComponent<Rigidbody>();
			Destroy(rigidbody, 1);
		}
	}

	//public float rotateForce = 10;
	//private void FixedUpdate()
	//{

	//	//rigidbody.AddTorque(-rotateForce * Time.fixedDeltaTime * transform.rotation.eulerAngles);

	//	if (rigidbody && rigidbody.IsSleeping())
	//		rigidbody.isKinematic = true;
	//}
	public float cost;
}
