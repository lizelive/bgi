using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mc
{
    public class BlockStates
    {
        public string name;
        // note does not have to be an array.
        public Dictionary<string, MaybeMcModelDescriptorArray> variants;
        public MulitpartCase[] multipart;

    }

    [Serializable]
    public class MulitpartCase
    {
        public MulitpartCondition when;
        public MaybeMcModelDescriptorArray apply;
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




    public class McRotation
    {
        public enum Axis
        {
            x, y, z
        }

        public Vector3 AxisVec
        {
            get
            {
                switch (axis)
                {
                    case Axis.x:
                        return Vector3.right;
                    case Axis.y:
                        return Vector3.up;
                    case Axis.z:
                        return Vector3.forward;
                    default:
                        return Vector3.zero;
                }
            }
        }

        public float angle;
        public float[] origin;
        public Axis axis;
        public bool rescale;

        public Vector3 Origin => origin.ToVec3() / 16 - Vector3.one / 2;

        public Matrix4x4 matrix =>
            Matrix4x4.Translate(Origin)
            * Matrix4x4.Rotate(Quaternion.AngleAxis(angle, AxisVec))
            * Matrix4x4.Translate(-Origin);
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
        public McRotation rotation;
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
        _0 = 0,
        _90 = 90,
        _180 = 180,
        _270 = 270
    }
    [Serializable]
    public enum Cullface
    {
        down, up, north, south, west, east
    }

    [Serializable]
    public partial class Face
    {
        public float[] Uv;
        public string Texture;
        public string Cullface;
        public int? TintIndex;
    }

}
