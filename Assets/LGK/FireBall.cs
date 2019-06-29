using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Health by;
    public GameObject explosion;
    private void OnCollisionEnter(Collision collision)
    {
        //print($"You just fireballed the {collision.gameObject.name}");

        ShitsOnFireYo.Burn(collision.gameObject, by);
        
        if (explosion)
            Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(gameObject, 1);
    }
}
