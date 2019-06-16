using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject explosion;
    private void OnCollisionEnter(Collision collision)
    {
        print($"You just fireballed the {collision.gameObject.name}");


        ShitsOnFireYo.Burn(collision.gameObject);

        var playWithFire = collision.gameObject.GetComponentInParent<IBurnable>();
        playWithFire?.Burn();
        if (explosion)
            Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject, 1);
    }
}
