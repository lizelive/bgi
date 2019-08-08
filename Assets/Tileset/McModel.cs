using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Mc
{
    public class BlockStates
    {

        // note does not have to be an array.
        public Dictionary<string, McModelDescriptor[]> variants;

        public Dictionary<string, McModelDescriptor> variants2;
        public McModelDescriptor[] multipart;

    }


    class MulitpartCondition
    {
        public MulitpartCondition[] OR, AND;
        public Dictionary<string, string> value;
    }
    class MulitpartWhatever
    {
        public object when;
        public McModelDescriptor apply;
    }
    [Serializable]

    public class McModelDescriptor
    {
        public string model;
        public int y;
        public int x;
        public bool uvlock;
        public int weight;
    }

    [Serializable]
    public partial class McModel
    {
        public string ambientocclusion;
        public string parent;
        public Dictionary<string, string> textures;
        public Element[] elements;
        public Display display;


    }
    [Serializable]
    public partial class Display
    {
        public Transform fhirdpersonRighthand;
        public Transform fhirdpersonLefthand;
        public Transform firstpersonRighthand;
        public Transform firstpersonLefthand;
        public Transform gui;
        public Transform ground;
        public Transform @fixed;
    }

    [Serializable]
    public partial class Transform
    {
        public float[] rotation;
        public float[] translation;
        public float[] scale;
    }

    [Serializable]
    public partial class Element
    {
        public float[] from;
        public float[] to;
        public Faces faces;
    }

    [Serializable]
    public partial class Faces
    {
        public Face down;
        public Face up;
        public Face north;
        public Face south;
        public Face west;
        public Face east;
    }

    [Serializable]
    public enum Rotation
    {
        _0=0,
        _90=90,
        _180=180,
        _270=270
    }
    [Serializable]
    public enum Cullface
    {
        down,up,north,south,west,east
    }

    [Serializable]
    public partial class Face
    {
        public float[] Uv;
        public string Texture;
        public string Cullface;
    }

}
