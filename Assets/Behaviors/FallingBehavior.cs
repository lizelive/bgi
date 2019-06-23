using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class FallingBehavior : AiBehavior
{
    private void OnCollisionEnter(Collision collision)
    {
        if (Running)
            Me.SwitchBehavior();
    }
}
