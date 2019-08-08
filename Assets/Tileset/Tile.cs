using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        //public object data;
        public override string ToString()
        {
            return $"Tile {blocktype}";
        }
        public Block Block => Block.Types[blocktype];
        public Mesh Mesh => Block.mesh;
        public bool IsAir => blocktype == 0;

        public bool IsSolid => !IsAir;

        public static BlockState Air => new BlockState();

        public static implicit operator bool(BlockState d) => !d.IsAir;


        bool BlockUpdate()
        {
            return false;
        }
    }
