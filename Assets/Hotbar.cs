using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public Player player;
    public MeshFilter currentBlockFilter;
    // Start is called before the first frame update
    void Start()
    {
        currentBlockFilter = currentBlockFilter ?? GetComponentInChildren<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        currentBlockFilter.mesh = player.grid.build.Mesh;
    }
}
