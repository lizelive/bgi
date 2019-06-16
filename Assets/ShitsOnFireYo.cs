using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitsOnFireYo : MonoBehaviour
{
    public static void Burn(GameObject thing)
    {
        var lol = Instantiate(Default.YoOnFire, thing.transform, true);
        lol.transform.localPosition = Vector3.zero;

    }

    private Health hurts;
    public float dps = 10;
    public float durration = 10;


    private float start;

    private ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        start = Time.time;
        hurts = GetComponentInParent<Health>();

        if (!hurts) {
            Destroy(this);
                }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - start > durration)
        {
            Destroy(gameObject);
        }
        else
        {
            hurts.Hurt(dps * Time.deltaTime, DamageKind.Fire);
        }
    }
}
