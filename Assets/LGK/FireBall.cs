using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FireBall : MonoBehaviour
{
	public Health by;
	public GameObject explosion;
	public float radius = 2;


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.GetComponentInChildren<Health>() == by)
			return;


		ShitsOnFireYo.Burn(collision.gameObject, by);

		foreach (var hit in
		Physics.OverlapSphere(transform.position, radius))
		{
			ShitsOnFireYo.Burn(hit.gameObject, by);

		}


		if (explosion)
			Instantiate(explosion, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}
}
