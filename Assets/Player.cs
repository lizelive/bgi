using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Hate Points
    /// </summary>
    public Health Health
    {
        get
        {
            if (health)
                return health;

            return  health = GetComponent<Health>();
        }
    }
    private Health health;
    public Swarm squad;
    public MeleeWeapon weapon;
    public Norb NorbPrefab;

    public Transform targeter;
    public ParticleSystem caller;

    // Start is called before the first frame update
    void Start()
    {
        weapon = weapon ?? GetComponentInChildren<MeleeWeapon>();
        squad = GetComponent<Swarm>();
        Health.OnHurt += Health_OnHurt;

        var shape = caller.shape;
        shape.radius = maxCallRange;
    }

    private void Health_OnHurt(Health by)
    {
        var followers = Followers.ToArray();
        if (followers.Any())
        {
            var savemesenpi = followers.Random();
            savemesenpi.SetJob(new Job(JobKind.Attack, by.gameObject));

        }
    }

    public float LaunchVel = 3;

    public float NorbCost = 5;

    public float norbGrabRange = 2;
    public float maxCallRange = 5;
    public GameObject followPoint;

    public IEnumerable<Norb> Followers => FindNearbyOwnedNorbs(norbGrabRange);


    public IEnumerable<Norb> FindNearbyOwnedNorbs(float range)
    {
        return FindObjectsOfType<Norb>().Where(norb => norb.owner == this && norb.IsGrounded && this.Distance(norb) < range);
    }
    


    // Update is called once per frame
    void Update()
    {

        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            targeter.position = hit.point;
        }
        else
        {

        }


        var doThrow = Input.GetButtonDown("Fire1");
        var doSummon = false && Input.GetButtonDown("Fire2");
        var doLocationController = Input.GetKey(KeyCode.Mouse2);
        var doWhistle = Input.GetKey(KeyCode.R);

        followPoint.transform.position = doLocationController ? targeter.position : transform.position - transform.forward;

        caller.gameObject.SetActive(doWhistle);

        if (doThrow)
        {

            var noob = Followers.MinBy(n => this.Distance(n));
            if (noob)
            {
                var startPos = transform.position + transform.forward + Vector3.up;
                var targetPos = targeter.position;


                var y = targetPos.y-startPos.y;

                var startPos2d = startPos.xz();
                var targetPos2d = targetPos.xz();

                var distance = Vector2.Distance(startPos2d, targetPos2d);

                var gravity = 9.81f;

                var v = LaunchVel;
                var v2 = v * v;
                var d2 = Mathf.Pow(distance, 2);

                var y2 = Mathf.Pow(y, 2);

                //var vy0 = -Mathf.Sqrt(-(2*d2*g*y) / (d2 + y2) + (d2*LV)/ (d2 + y2) + (2*LV*y2)/ (d2 + y2) - Mathf.Sqrt(-d ^ 4(4*d2*g2 - 4*g*LV*y - Mathf.Pow(LV, 2))) / (d ^ 2 + y ^ 2))/ sqrt(2)

                
                var theta = Mathf.Atan2(v2 + Mathf.Sqrt(v2 * v2 - gravity * (gravity * d2 + 2 * y * v2)), gravity*distance);
                var vy = Mathf.Sin(theta)*v;
                var vx = Mathf.Cos(theta) * v;

                var launchVel = Vector3.up * vy + vx * (targetPos2d - startPos2d).normalized.x0y();


                noob.transform.position = startPos;
                noob.Throw(launchVel);




            }
        }
        if (doSummon)
        {
            Health.Hurt(NorbCost, DamageKind.Sacrifice, null, Health);
            var noob = Instantiate(NorbPrefab, transform.position + transform.forward+Vector3.up, Quaternion.identity);
            noob.GetComponent<Health>().team = Health.team;
            noob.owner = this;
        }

        if (doWhistle)
        {

            var nearby = FindObjectsOfType<Norb>().Where(norb => norb.owner == this && norb.IsGrounded && targeter.Distance(norb) < maxCallRange);
            
            foreach (var norb in nearby)
            {
                norb.FollowPlayer();
            }
        }

    }
}
