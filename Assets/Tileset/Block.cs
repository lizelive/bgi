using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Prop
{
    public string name;

    // option to have name
    // maybe use enum insted?
    public string[] values;



    // automaticly decided based of values.
    public byte size;

    // bit location. this should be automatic
    public byte position;
}

// might want to rethink this...
public class Block
{
    public string package;
    public string id;

    public ushort idnum;

    public Mesh mesh;

    public Prop[] props;
    public string[] tags;

    public Prop GetProp(string name)
    {
        return props.FirstOrDefault(x => x.name == name);

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


    public BlockState SetTag(BlockState state, string tag, string valueName)
    {
        var t = GetProp(tag);
        if (t == null)
            throw new ArgumentOutOfRangeException("Tag not found");

        byte value;

        if (!byte.TryParse(valueName, out value))
            for (int i = 0; i < t.values.Length; i++)
            {
                if (t.values[i] == valueName)
                {
                    value = (byte)i;
                    break;
                }
            }
        state.magicNumber = state.magicNumber.Bits(t.position, t.size, value);
        return state;
    }


    public string ToPropString(BlockState state)
    {
        if (props is null)
            return $"{package}:{id}";
        else
            return $"{package}:{id}[{string.Join(",", props.Select(p => $"{p.name}={GetTag(state, p.name)}"))}]";
    }


    public BlockState ParseState(string data)
    {
        var block = this;
        var parts = data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var state = new BlockState
        {
            blockId = block.idnum
        };

        for (int i = 0; i < parts.Length; i++)
        {
            var propAssign = parts[i].Trim().Split('=');
            if (propAssign.Length != 2)
            {
                throw new InvalidOperationException("Data is bad.");
            }

            var propName = propAssign[0].Trim();
            var propValString = propAssign[1].ToLowerInvariant().Trim();
            var prop = block.GetProp(propName);
            state = block.SetTag(state, propName, propValString);
        }
        return state;
    }

    public static BlockState Parse(BlockState state, string data)
    {
        var block = state.Block;
        var parts = data.Split(',');
        for (int i = 0; i < parts.Length; i++)
        {
            var propAssign = parts[i].Trim().Split('=');
            if (propAssign.Length != 2)
            {
                throw new InvalidOperationException("Data is bad.");

            }

            var propName = propAssign[0].Trim();
            var propValString = propAssign[1].ToLowerInvariant().Trim();
            var prop = block.GetProp(propName);
            var propValue = byte.Parse(propValString);
            state = block.SetTag(state, propName, propValue);
        }
        return state;
    }

    public static BlockState Parse(string data)
    {
        var parts = data.Split('[', ',', ']');
        var blockname = parts[0].Trim();
        var block = BlockRepo.Get(blockname);

        var state = new BlockState();
        state.blockId = block.idnum;

        for (int i = 1; i < parts.Length - 1; i++)
        {
            var propAssign = parts[i].Trim().Split('=');
            if (propAssign.Length != 2)
            {
                throw new InvalidOperationException("Data is bad.");

            }


            var propName = propAssign[0].Trim();
            var propValString = propAssign[1].ToLowerInvariant().Trim();
            var prop = block.GetProp(propName);
            var propValue = byte.Parse(propValString);
            state = block.SetTag(state, propName, propValue);




        }


        return state;
    }


    public bool isAir;
    public bool isSolid;

    public Mc.BlockStates blockStates;
    public Mesh[][] render;

    public void RecomputeAssets(McRespack respack)
    {
    }

    //public Mesh GetMesh(BlockState state, Vector3Int pos, VoxelWorld world)
    //{
    //    return meshes[state.magicNumber];
    //}
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
    public static Dictionary<string, Block> blockById;
    public static Block[] blocks;

    static BlockRepo()
    {

        //TODO don't do this. please do literly anything else.
        var path = @"C:\Users\Lize\source\bgi\Assets\Tileset\blocks.json";
        var json = System.IO.File.ReadAllText(path);
        blocks = Newtonsoft.Json.JsonConvert.DeserializeObject<Block[]>(json);
        for (ushort i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            block.idnum = i;
        }

        blockById = blocks.ToDictionary(x => x.id, x => x);

    }

    public static Block Get(string id)
    {
        if (blockById.TryGetValue(id, out var block))
        {
            return block;
        }
        return null;
    }

    public static Block Get(int id)
    {
        if (id < blocks.Length)
            return blocks[id];
        return null;
    }
}
