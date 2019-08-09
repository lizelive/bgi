using System;
using UnityEngine;

[Serializable]
public struct BlockState
{
    public short blockId;
    public byte magicNumber;
    private static readonly Mesh EmptyMesh = new Mesh();
    //public object data;
    public override string ToString()
    {
        return $"Tile {blockId}";
    }
    //public Block Block => Block.Types[blocktype];
    //public Mesh Mesh => Block.mesh;
    public Mesh Mesh
    {
        get
        {
            if (blockId == 1)
            {
                return Default.I.models[220];
            }
            else if (blockId > 0)
            {
                return Default.I.models[blockId - 2];
            }
            else
            {
                return EmptyMesh;
            }
        }

    }
    public bool IsAir => blockId == 0;

    public bool IsSolid => !IsAir;

    public static BlockState Air => new BlockState();

    public static implicit operator bool(BlockState d) => !d.IsAir;


    bool BlockUpdate()
    {
        return false;
    }
}
