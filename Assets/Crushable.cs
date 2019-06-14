using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Crushable : MonoBehaviour
{
    public float crushWeight = 10f;
    public TextMeshPro text;
    

    // Update is called once per frame
    void Update()
    {
        if (weightOnMe >= crushWeight)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        if(text)
        text.text = $"{weightOnMe}/{crushWeight}";
    }

    public float weightOnMe = 0;

    private void OnCollisionEnter(Collision collision)
    {
        weightOnMe += collision.rigidbody.mass;
    }

    private void OnCollisionExit(Collision collision)
    {
        weightOnMe -= collision.rigidbody.mass;
    }
}
