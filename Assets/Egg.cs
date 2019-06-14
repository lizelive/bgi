using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public Norb spawns;
    new Rigidbody rigidbody;
    public Player owner;
    public GameObject effect;

    float startTime;
    public float hatchTime = 10;
    public float hatchDist = 10;
    public float hatchLength = 3;
    float distTravled;
    float startHatchTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        rigidbody = GetComponent<Rigidbody>();
        lastPos = this.pos();
    }
    private bool hatched = false;
    // Update is called once per frame
    Vector3 lastPos;

    void Update()
    {
        distTravled += Vector3.Distance(lastPos, lastPos = this.pos());

        var age = Time.time - startTime;
        if(!hatched && age > hatchTime && distTravled > hatchDist)
        {
            hatched = true;
            effect.SetActive(true);
            startHatchTime = Time.time;
        }

        if (hatched && Time.time - startHatchTime > hatchLength)
        {
            print("egg hatching");
            var norb = Instantiate(spawns, transform.pos(), Quaternion.identity);
            norb.owner = owner;
            Destroy(gameObject);
        }

    }
}
