using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default : MonoBehaviour
{
    public static ShitsOnFireYo YoOnFire => I.yoOnFire;

    public ShitsOnFireYo yoOnFire;

    static Default I;

    public Default()
    {
        I = this;
    }

}
