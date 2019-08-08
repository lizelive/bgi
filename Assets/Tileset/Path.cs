using UnityEngine;

public class NavPath
{
    public Vector3Int Start => steps[0];
    public Vector3Int End => steps[steps.Length - 1];
    public Vector3Int[] steps;

    public NavPath(Vector3Int[] steps)
    {
        this.steps = steps;
    }
}