using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodAlter : MonoBehaviour
{
	public bool enabled = true;
	public float BloodRate = 1;
	public ParticleSystem particles;
	public Health helping;
    // Start is called before the first frame update
    void Start()
    {
		particles = GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
		helping.Heal(BloodRate * Time.deltaTime);   
    }
}
