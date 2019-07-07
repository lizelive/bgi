using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Default : MonoBehaviour
{
    public static ShitsOnFireYo YoOnFire => I.yoOnFire;

	public GameObject TargetIcon;
    public ShitsOnFireYo yoOnFire;
	public Controls controls;

	public string playerName;

    public static Default I;

    public Default()
    {
        I = this;
    }

}
