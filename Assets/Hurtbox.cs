using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public object sadist;
    public bool once;
    public float damage;
    new BoxCollider collider;

   
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float hitdamage; 

        if (once)
        {
            hitdamage = damage;
            enabled = false;
        }
        else
        {
            hitdamage = damage * Time.deltaTime;
        }

        var hurting = Physics.OverlapBox(collider.center, collider.size / 2, transform.rotation);
        foreach (var hurtable in hurting)
        {
            var health = GetComponentInChildren<Health>();
        }

    }
}
