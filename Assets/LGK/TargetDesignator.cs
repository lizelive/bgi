using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetDesignator : MonoBehaviour
{
	public float Weight =1 ;
	public bool NeedLineOfSight = true;
	public Team team;
	public TargetKind Kind;

	public Vector3 Pos => transform.position;
}
