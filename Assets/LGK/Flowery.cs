using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Flowery : MonoBehaviour
{
    public float smells = 3;
    public float lastSmelt;
    public bool IsValid => Time.time - lastSmelt > 3 * smells;
}
