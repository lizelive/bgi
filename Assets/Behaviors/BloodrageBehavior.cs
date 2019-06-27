using UnityEngine;

public class BloodrageBehavior : AiBehavior
{
    public Mob IWannaKill;

    public override bool ComeFromAny => true;
    public override float CurrentPriority => onLookout && IsVisible() ? BasePriority : 0;
    public float speed = 12.42771f;

    public bool onLookout = true;

	public void Start()
	{
		IWannaKill = FindObjectOfType<Player>()?.GetComponent<Mob>();
	}


	public bool IsVisible()
    {
        if (!IWannaKill) return false;
        var dir = IWannaKill.pos() - Me.pos();
        var dist = dir.magnitude;
        dir.Normalize();

        if (
            Physics.Raycast(Me.head.position+dir, dir, out var hit)
            //&& hit.distance <= Me.ViewRange
            && (Mathf.Abs(dist - hit.distance) < 2 || hit.distance > dist))
        {
            return true;
        }
        return false;
    }
    public override void Run()
    {
        // we killed him...
        if (!IWannaKill)
        {
            End();
            return;
        }


        if (!IsVisible())
        {
            onLookout = true;
            End();
            return;
        }

        var dir = (IWannaKill.pos() - Me.pos()).normalized;

        // use pid loop to prevent infinate force
        Me.SetVelocity(dir* speed);

        if(Me.Distance(IWannaKill) < 2)
        {
            End();
        }

    }


    public AudioClip leroooy;

    // Update is called once per frame
    void Update()
    {
        if (onLookout && IsVisible())
        {
            onLookout = false;
            GetComponent<AttackBehavior>().target = IWannaKill.Health;
            Me.SwitchBehavior<BloodrageBehavior>();
            

            GetComponent<AudioSource>().Play();

            //AudioSource.PlayClipAtPoint(leroooy, transform.position);
            //AudioMan.I.Play("hero/leroy", transform.position);
            //enabled = false;
        }
    }
}
