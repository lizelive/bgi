using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Team team;
    public GameObject explosion;
    private void OnCollisionEnter(Collision collision)
    {
        //print($"You just fireballed the {collision.gameObject.name}");

        ShitsOnFireYo.Burn(collision.gameObject, team);
        
        if (explosion)
            Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(gameObject, 1);
    }
}
