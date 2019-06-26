using UnityEngine;

public class Villager : MonoBehaviour
{
    public AudioClip oof;
    // Start is called before the first frame update
    void Start()
    {
        var mob = GetComponent<Mob>();
        mob.Health.OnHurt += Health_OnHurt;
        mob.Health.OnDie += Health_OnDie;
    }

    private void Health_OnDie(Health obj)
    {
        //if (obj.GetComponent<Player>())
        {
            VillageController.I.KilledAVillager();
        }
    }

    private void Health_OnHurt(Health obj)
    {
        if (oof)
            AudioSource.PlayClipAtPoint(oof, transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
