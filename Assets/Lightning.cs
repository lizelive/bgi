using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public new LineRenderer renderer;
    public UnityEngine.Transform target;
    
    public float randomRange = 2;
    // Update is called once per frame
    void Update()
    {

        if (!target)
            return;

        var pos = transform.pos();
        var tpos = target.pos();

        var disp = (tpos - pos);
        var dir = disp.normalized;
        var dist = disp.magnitude;
        var stepToTarget = dist / renderer.positionCount;
        var randomRangeAmt = stepToTarget * randomRange;


        var poss =  new  Vector3[renderer.positionCount];
        for (int i = 0; i < renderer.positionCount; i++)
        {
            poss[i]= pos += Random.insideUnitSphere * randomRangeAmt + dir * stepToTarget;
           
        }
        renderer.SetPositions(poss);
    }
}
