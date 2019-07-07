using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum JobTypes
{
	None,
	Murder,
	Destroy,
	Build,
	Opperate,
	Carry,
	Other,
	Count
}

public class JobRunnerBase<T> where T : IJob
{
	public T Job;
	public Mob Me;

	void Start()
	{

	}


	public virtual void OnStart()
	{
		Me.SetTarget(Me.transform);
	}
	public virtual void OnEnd()
	{

	}
	public virtual void OnRun()
	{

	}
}

public enum JobStatus
{
	Invalid,
	Hiring,
	Running,
	Ended
}

public abstract class BaseJob : IJob
{
	HashSet<Mob> workers = new HashSet<Mob>();

	public virtual JobTypes kind => JobTypes.None;
	public virtual float BasePriority => 0;

	public virtual bool IsHiring => Workers.Count() < MaxNumWorkers;
	public virtual string WorkerType => "Glorious";
	public virtual int MinNumWorkers => 1;
	public virtual int MaxNumWorkers => 10;
	public virtual string Name => this.GetType().Name.Replace("Job", "");
	public IEnumerable<Mob> Workers => workers;
	public virtual bool IsComplete => true;

	public float Currentpriority => BasePriority / Workers.Count();
	public virtual void OnRun(Mob me) { }

	public virtual bool IsValidMob(Mob mob) => !!mob;
	public bool Register(Mob me)
	{
		if (IsValidMob(me) && workers.Add(me))
		{
			me.job = this;
			OnRun(me);
			return true;
		}
		return false;
	}
	public bool UnRegister(Mob me)
	{
		me.job = null;
		return workers.Remove(me);
	}
	public virtual void OnRun() { }

	public int CompareTo(object obj)
	{

		var job = obj as IJob;
		if (job == null)
			throw new Exception("Wtf?");
		return Currentpriority.CompareTo(job.Currentpriority);
	}

	public virtual void Start()
	{

	}

	public virtual void Finish()
	{
		foreach (var mob in Workers)
		{
			mob.job = null;
			mob.SwitchBehavior<IdleBehavior>();
		}
	}
}

public class MurderJob : BaseJob
{
	public Health target;
	public override JobTypes kind => JobTypes.Destroy;
	public override float BasePriority => 30;
	public override bool IsHiring => !!target;
	public override string Name => $"Murder {target.name}";
	public override int MaxNumWorkers => 100;


	public override bool IsValidMob(Mob mob) => mob.Behaviors.Any(x => x is AttackBehavior);

	public override bool IsComplete => !target;

	public override void OnRun(Mob me)
	{
		var jfk = target;
		me.GetComponent<AttackBehavior>().target = jfk;
		var behavior = me.SwitchBehavior<AttackBehavior>();
	}

	public override bool Equals(object obj)
	{
		var job = obj as MurderJob;
		return job != null &&
			   EqualityComparer<Health>.Default.Equals(target, job.target);
	}



	private GameObject targetIcon;

	public MurderJob(Health target)
	{
		this.target = target;
	}

	public override void Finish()
	{
		base.Finish();
		GameObject.Destroy(targetIcon);
	}

	public override void Start()
	{

		targetIcon = GameObject.Instantiate(
			Default.I.TargetIcon,
			target.transform);

		targetIcon.transform.localPosition = Vector3.up * 3;
		base.Start();
	}

	public override int GetHashCode()
	{
		return -962741368 + EqualityComparer<Health>.Default.GetHashCode(target);
	}
}

//public class JobCompare : IComparer<IJob>

public interface IJob : IComparable
{
	JobTypes kind { get; }
	float BasePriority { get; }
	bool IsHiring { get; }
	string WorkerType { get; }
	int MinNumWorkers { get; }
	int MaxNumWorkers { get; }
	string Name { get; }
	bool IsValidMob(Mob mob);
	bool Register(Mob me);
	bool UnRegister(Mob me);
	IEnumerable<Mob> Workers { get; }
	bool IsComplete { get; }
	float Currentpriority { get; }

	void Finish();
	void Start();
}

public class JobManager : MonoBehaviour
{

	public IEnumerable<Mob> IdleMob => team.mobs.Where(m => m.ActiveBehavior is IdleBehavior);
	public List<IJob> openJobs = new List<IJob>();

	public Team team;

	public void Awake()
	{
		team.JobManager = this;
	}

	public bool UnpublishJob(IJob job)
	{
		var remove = openJobs.Find(job.Equals);

		if (remove == null)
			return false;

		openJobs.Remove(remove);
		remove.Finish();
		return true;
	}

	public void ClearAllJobs()
	{
		openJobs.Select(UnpublishJob);
	}

	public void PublishJob(IJob job)
	{
		if (!openJobs.Contains(job))
		{
			openJobs.Add(job);
			job.Start();
		}
	}

	// Update is called once per frame
	void Update()
	{
		foreach (var job in openJobs.Where(x => x.IsComplete).ToArray())
		{
			UnpublishJob(job);
		}


		//var sortedJobs = openJobs.OrderBy(x => x.Currentpriority).ToList();

		var sortedJobs = new SortedSet<IJob>(openJobs.Where(j => j.IsHiring));

		var idleMobs = IdleMob.ToArray();

		foreach (var yoma in idleMobs)
		{
			var myJob = sortedJobs.FirstOrDefault(x => x.IsValidMob(yoma));
			if (myJob != null && myJob.Register(yoma))
			{
				sortedJobs.Remove(myJob);
				if (myJob.IsHiring)
					sortedJobs.Add(myJob);
			}
		}

	}
}
