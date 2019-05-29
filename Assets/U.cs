using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class U
{
    public static float Distance(this GameObject self, GameObject other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }
    public static float Distance(this Component self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }
    public static float Distance(this GameObject self, Component other)
    {
        return Vector3.Distance(self.transform.position, other.transform.position);
    }
}
