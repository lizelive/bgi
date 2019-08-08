using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FarmBehavior : AiBehavior
{
    public float priority;

    public float MaxRange;
    public float HarvestSpeed = 1;
    public Plant plant;
    public float harvestStartTime = float.PositiveInfinity;
	public override bool SwitchToAny => true;

	public override bool OnBegin()
    {
        if(!plant)
        {
            plant= gameObject.Find<Plant>(Me.ViewRange).Where(p=>p.Ready).Random();
        }

        if (plant==null || !plant)
        {
            return false;
        }
		plant.lastLookat = Time.time;

		Me.SetTarget(plant.transform, plant.GetComponent<Health>().radius + Me.Health.radius + 1);

        return base.OnBegin();
    }

    public override float CurrentPriority => BasePriority;

    public override bool OnEnd()
    {

        Me.Animator.SetBool("PickFruit", false);
        harvestStartTime = float.PositiveInfinity;
        plant = null;
        return base.OnEnd();
    }

    public float WorthIt = 0.1f;

    public override void Run()
    {
		if (!plant)
        {
            Me.SwitchBehavior();
            return;
        }
		//Me.SetTarget(plant.transform, 2);

		plant.lastLookat = Time.time;


		if (Me.AtTarget)
        {
			Debug.DrawLine(transform.position, plant.transform.position, Color.magenta);
            Me.Animator.SetBool("PickFruit", true);

            var got = plant.Harvest(plant.harvestPerSecond * Time.deltaTime);

            VillageController.I.DepositFood(got);

            if (plant.PGrowth <= WorthIt)
            {
               harvestStartTime = float.PositiveInfinity;
                plant = null;
                Me.SwitchBehavior<DepositBehavior>();
            }
        }
    }

}
