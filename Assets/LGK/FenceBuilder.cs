using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
	public Building fencePrefab;
	public UnityEngine.Transform targeter;

	public float fenceLength = 1;
    // Start is called before the first frame update
    
		
	public Vector3 lastPost;

    // Update is called once per frame
    void Update()
    {
		//fenceLength = fencePrefab.GetComponent<Health>().radius;

		if (Input.GetKeyDown(KeyCode.J))
		{
			lastPost = targeter.position;
		}

		if (Input.GetKey(KeyCode.J))
		{
			var curPos = targeter.position;
			if(Vector3.Distance(lastPost, curPos) >= fenceLength) {
				var direction = (curPos - lastPost).normalized;
				var nextPost = lastPost + direction * fenceLength;
				var dir = Vector3.Cross(direction, Vector3.up);
				var rot = Quaternion.LookRotation(dir);
				var pos = (nextPost + lastPost) / 2;
				var noob = Instantiate(fencePrefab, transform);
				noob.transform.parent = transform;
				noob.transform.position = pos;
				noob.transform.rotation = rot;

				//var rot = Quaternion.FromToRotation(lastPost, nextPost);

				lastPost = nextPost;
			}
		}


	}
}
