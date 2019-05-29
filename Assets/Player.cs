using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Hate Points
    /// </summary>
    public Health health;
    public Swarm squad;
    public MeleeWeapon weapon;
    public Norb NorbPrefab;
    // Start is called before the first frame update
    void Start()
    {
        weapon = weapon ?? GetComponentInChildren<MeleeWeapon>();
        health = GetComponent<Health>();
        squad = GetComponent<Swarm>();
    }

    // Update is called once per frame
    void Update()
    {
        var fire1 = Input.GetButtonDown("Fire1");
        var fire2 = Input.GetButtonDown("Fire2");


        if (fire1)
            weapon.Attack();
        if (fire2)
        {
            var noob = Instantiate(NorbPrefab, transform.position + transform.forward, Quaternion.identity);
            noob.GetComponent<Health>().team = health.team;
            noob.owner = this;
        }

    }
}
