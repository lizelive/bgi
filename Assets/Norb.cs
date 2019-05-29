using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Norb : MonoBehaviour
{
    public Material mat;
    public Color color;
    // Start is called before the first frame update
    void Start()
    {
        var meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var mesh in meshes)
        {
            mesh.material.color = color;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
