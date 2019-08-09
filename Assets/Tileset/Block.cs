using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using System;


public class Prop
{
    public string name;
    public byte size;
    
    // option to have name
    // maybe use enum insted?
    public string[] names;
    // bit location
    public byte position;
}

// might want to rethink this...
public class Block
{
    public string package;
    public string id;

    public Mesh mesh;

    public Prop[] dataTags;
    public string[] tags;

    public Prop GetProp(string name)
    {
        return dataTags.FirstOrDefault(x => x.name == name);

    }

    public int GetTag(BlockState state, string tag)
    {
        var t = GetProp(tag);
        if (t == null)
            throw new ArgumentOutOfRangeException("Tag not found");
        return state.magicNumber.Bits(t.position, t.size);
    }

    public BlockState SetTag(BlockState state, string tag, byte value)
    {
        var t = GetProp(tag);
        if (t == null)
            throw new ArgumentOutOfRangeException("Tag not found");
        state.magicNumber = state.magicNumber.Bits(t.position, t.size, value);
        return state;
    }

    public BlockState Parse(string data, VoxelWorld world)
    {
        var parts = data.Split('[',',',']');
        var blockname = parts[0].Trim();
        var blockId = world.blockIds[blockname];

        var state = new BlockState();
        state.blockId = (short)blockId;


        Block block = new Block();

        for (int i = 1; i < parts.Length - 1; i++)
        {
            var propAssign = parts[i].Trim().Split('=');
            if (propAssign.Length != 2)
            {
                throw new InvalidOperationException("Data is bad.");
                
            }


            var propName = propAssign[0].Trim();
            var propValString = propAssign[1].ToLowerInvariant().Trim();
            var prop = GetProp(propName);
            var propValue = byte.Parse(propValString);
            state = block.SetTag(state, propName, propValue);




        }


        return state;
    }


    public bool isAir;
    public bool isSolid;

    public Mc.BlockStates blockStates;

    private readonly Mesh[] meshes = new Mesh[256];

    public void RecomputeAssets(McRespack respack)
    {
    }

    public Mesh GetMesh(BlockState state, Vector3Int pos, VoxelWorld world)
    {
        return meshes[state.magicNumber];
    }
}

interface IBlockData
{
    void FromMagic(byte magicNumber);
    byte AsMagic(byte magic);
    void FromString(string data);
    string AsString();
}

public class BlockRepo
{
    Dictionary<string, Block> blocks;

}
