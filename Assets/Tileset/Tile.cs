using System;
using UnityEngine;


// might want to rethink this...
public class Block
{

    public string name;
    public Mesh mesh;
    public bool isAir;
    public bool isSolid;


    public static Block[] Types => new[]
    {
        new Block { name = "air", mesh = new Mesh() },
        new Block { name = "stone", mesh = Default.I.buildingBlockMesh}
    };


}

public interface IBlockData
{
    string GetAsString();
}

[Serializable]
public struct BlockState
{
    public IBlockData data;
    public short blocktype;
    private static readonly Mesh EmptyMesh = new Mesh();
    //public object data;
    public override string ToString()
    {
        return $"Tile {blocktype}";
    }
    public Block Block => Block.Types[blocktype];
    //public Mesh Mesh => Block.mesh;
    public Mesh Mesh
    {
        get
        {
            if (blocktype == 1)
            {
                return Default.I.buildingBlockMesh;
            }
            else if (blocktype > 0)
            {
                return Default.I.models[blocktype - 2];
            }
            else
            {
                return EmptyMesh;
            }
        }

    }
    public bool IsAir => blocktype == 0;

    public bool IsSolid => !IsAir;

    public static BlockState Air => new BlockState();

    public static implicit operator bool(BlockState d) => !d.IsAir;


    bool BlockUpdate()
    {
        return false;
    }
}
