using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitsOnFireYo : MonoBehaviour
{
    public static void Burn(GameObject thing, Health by = null)
    {


        thing.GetComponentInParent<IBurnable>()?.Burn();



        var health = thing.GetComponent<Health>();

        if (!health || !by.team.Fighting(health.team))
        {
            return;
        }

        var lol = thing.GetComponentInChildren<ShitsOnFireYo>();
        if (lol)
        {
            lol.start = Time.time;
            return;
        }
        lol = Instantiate(Default.YoOnFire, thing.transform, true);
		lol.by = by;
		lol.transform.localPosition = Vector3.zero;

    }

    private Health hurts;
    public float dps = 10;
    public float durration = 10;

	public Health by;
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

			hurts.Hurt(dps * Time.deltaTime, DamageKind.Fire, by, allowFriendlyFire: true, ignoreCooldown: true);
        }
    }
}
