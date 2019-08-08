using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lecturn : MonoBehaviour
{
	public UnityEngine.Transform player;


	public UnityEngine.Transform start;
	public UnityEngine.Transform end;

	public float maxDistance = 3;
	public float minDistance = 1;
	public AnimationCurve distanceCurve;


	public UnityEngine.Transform moveMe;


	private void Start()
	{
		moveMe.localPosition = start.localPosition;
		moveMe.localRotation = start.localRotation;
		moveMe.localScale = start.localScale;
	}
	// Start is called before the first frame update
	void Update()
    {
		player = gameObject.Find<Player>(maxDistance).Closest(gameObject)?.transform;
		if (player)
		{
			var dist = this.Distance(player);
			var distp = Mathf.InverseLerp(maxDistance, minDistance, dist);
			distp = Mathf.Clamp01(distp);
			var lerp = distanceCurve.Evaluate(distp);

			moveMe.position = Vector3.Lerp(start.position, end.position, lerp);
			moveMe.rotation = Quaternion.Lerp(start.rotation, end.rotation, lerp);
			moveMe.localScale = Vector3.Lerp(start.localScale, end.localScale, lerp);
		}
	}
}
