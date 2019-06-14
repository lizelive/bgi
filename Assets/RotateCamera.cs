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
        var tp = camera.GetCinemachineComponent<CinemachineTransposer>();
        tp.m_FollowOffset = Quaternion.Euler(0, rot, 0) * tp.m_FollowOffset;
    }
}
