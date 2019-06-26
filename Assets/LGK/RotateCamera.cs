using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RotateCamera : MonoBehaviour
{
	public new CinemachineVirtualCamera camera;
	// Start is called before the first frame update
	void Start()
	{
		camera = GetComponent<CinemachineVirtualCamera>();
	}

	public float rotateBy = 45;
	// Update is called once per frame
	void Update()
	{
		float rot = 0;
		if (Input.GetKeyDown(KeyCode.Q))
			rot -= rotateBy;
		if (Input.GetKeyDown(KeyCode.E))
			rot += rotateBy;


		if (rot != 0)
		{
			var tp = camera.GetCinemachineComponent<CinemachineTransposer>();
			
			var rotation = Quaternion.Euler(0, rot, 0);
			tp.m_FollowOffset = rotation * tp.m_FollowOffset;
			Camera.main.transform.rotation = Quaternion.Euler(0, -rot, 0) * Camera.main.transform.rotation;
		}
	}
}
