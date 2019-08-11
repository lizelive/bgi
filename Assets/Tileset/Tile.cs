using System;
using UnityEngine;

[Serializable]
public struct BlockState
{
    public ushort blockId;
    public byte magicNumber;
    private static readonly Mesh EmptyMesh = new Mesh();


    public Block Block => BlockRepo.Get(blockId);

    //public object data;
    public override string ToString()
    {
        var b = Block;
        if (b != null)
            return b.ToPropString(this);
        return $"missinno {blockId} {magicNumber}";
    }
    //public Block Block => Block.Types[blocktype];
    //public Mesh Mesh => Block.mesh;
    public Mesh Mesh
    {
        get
        {
            if (blockId == 0)
            {
                return EmptyMesh;
            }
            var b = Block;
            if (!(b is null))
            {
                if (!(b is null || b.render is null))
                {
                    var opts = b.render[magicNumber];
                    if (!(opts is null))
                        return opts.Random() ?? EmptyMesh;
                }
            }

            if (blockId == 1)
            {
                return Default.I.models[220];
            }
            if (blockId > 1)
            {
                return Default.I.models[blockId - 2];
            }
            return Default.I.buildingBlockMesh;
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
