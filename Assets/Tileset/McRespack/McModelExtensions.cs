using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class McModelExtensions
{
    public static Mesh GetMesh(this Mc.McModel model, McRespack pack)
    {
        if (model?.elements is null)
            return null;
        var quads = model.elements.SelectMany(
            e => DirectionUtils.Directions.Select(d => e.GetQuad(model, pack, d))).Where(U.Is).ToArray();

        var mesh = new Mesh();

        if(quads.Length==0)
        {
            return mesh;
        }

        var indices = new int[quads.Length * 4];
        var verts = new Vector3[quads.Length * 4];
        var uv = new Vector2[quads.Length * 4];

        var numSubmeshes = quads.Max(x => x.submesh) + 1;
        List<int>[] indicess = new List<int>[numSubmeshes];
        for (int i = 0; i < indicess.Length; i++)
        {
            indicess[i] = new List<int>();
        }
        mesh.subMeshCount = indicess.Length;

        for (int i = 0; i < quads.Length; i++)
        {
            var b = i * 4;
            var q = quads[i];


            indicess[q.submesh].Add(b + 0);
            indicess[q.submesh].Add(b + 1);
            indicess[q.submesh].Add(b + 2);
            indicess[q.submesh].Add(b + 3);

            verts[b + 0] = q.v1;
            verts[b + 1] = q.v2;
            verts[b + 2] = q.v3;
            verts[b + 3] = q.v4;

            uv[b + 0] = q.uv1;
            uv[b + 1] = q.uv2;
            uv[b + 2] = q.uv3;
            uv[b + 3] = q.uv4;
        }

        mesh.vertices = verts;
        mesh.uv = uv;


        for (int i = 0; i < indicess.Length; i++)
        {
            mesh.SetIndices(indicess[i].ToArray(), MeshTopology.Quads, i);
        }

        //mesh.name = modelFile.FullName;

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        mesh.UploadMeshData(false);

        return mesh;
    }

    public static Vector3 ToVec3(this float[] data)
    {
        return new Vector3(data[0], data[1], data[2]);
    }

    public static Vector2 ToVec2(this float[] data)
    {
        return new Vector3(data[0], data[1], data[2]);
    }
    public static (Vector2, Vector2) ToVec2s(this float[] data)
    {
        return (new Vector2(data[0], data[1]), new Vector2(data[2], data[3]));
    }

    public static Mc.Face GetFace(this Mc.Faces self, Direction face)
    {
        switch (face)
        {
            case Direction.Down:
                return self.down;
            case Direction.Up:
                return self.up;
            case Direction.North:
                return self.north;
            case Direction.South:
                return self.south;
            case Direction.West:
                return self.west;
            case Direction.East:
                return self.east;
            default:
                break;
        }
        return null;
    }



    public static Quad GetFace(this Bounds self, Direction face)
    {
        var o = new Quad();


        var unw = new Vector3(self.min.x, self.max.y, self.min.z);
        var dnw = new Vector3(self.min.x, self.min.y, self.min.z);
        var usw = new Vector3(self.min.x, self.max.y, self.max.z);
        var dsw = new Vector3(self.min.x, self.min.y, self.max.z);
        var une = new Vector3(self.max.x, self.max.y, self.min.z);
        var dne = new Vector3(self.max.x, self.min.y, self.min.z);
        var use = new Vector3(self.max.x, self.max.y, self.max.z);
        var dse = new Vector3(self.max.x, self.min.y, self.max.z);


        switch (face)
        {
            case Direction.Down:
                o.v1 = dsw;
                o.v2 = dnw;
                o.v3 = dne;
                o.v4 = dse;
                break;
            case Direction.Up:
                o.v1 = unw;
                o.v2 = usw;
                o.v3 = use;
                o.v4 = une;
                break;
            case Direction.North:
                o.v1 = dne;
                o.v2 = dnw;
                o.v3 = unw;
                o.v4 = une;
                break;
            case Direction.South:
                o.v1 = dsw;
                o.v2 = dse;
                o.v3 = use;
                o.v4 = usw;
                break;
            case Direction.West:
                o.v1 = dnw;
                o.v2 = dsw;
                o.v3 = usw;
                o.v4 = unw;
                break;
            case Direction.East:
                o.v1 = dse;
                o.v2 = dne;
                o.v3 = une;
                o.v4 = use;
                break;
            default:
                break;
        }

        return o;
    }


    public static Vector2 Lerp(this Rect rect, Vector2 uv)
    {
        return rect.min + rect.size * uv;
    }

    public static Quad GetQuad(this Mc.Element element, Mc.McModel model, McRespack pack, Direction face)
    {
        var offset = -Vector3.one / 2;
        var from = element.from.ToVec3() / 16 + offset;
        var to = element.to.ToVec3() / 16 + offset;

        var mface = element.faces.GetFace(face);

        if (mface == null || mface.Uv == null) return null;

        var rot = element.rotation?.matrix ?? Matrix4x4.identity; 


        var texture = new Rect(0, 0, 1, 1);



        var texturePath = mface.Texture;


        model.textures?.TryGetValue(mface.Texture.TrimStart('#'), out texturePath);

        texturePath = texturePath ?? "";

        if (!pack.textures.TryGetValue(texturePath, out texture))
        {
            Debug.LogWarning($"Missing texture {mface.Texture} {texturePath}");
        }

        var (uv1, uv2) = mface.Uv.ToVec2s();
        uv1 /= 16;
        uv2 /= 16;

        uv1 = texture.Lerp(uv1);
        uv2 = texture.Lerp(uv2);


        var good = texture.Contains(uv1) && texture.Contains(uv2);

        var bounds = new Bounds(from, Vector3.zero);
        bounds.Encapsulate(to);

        var o = bounds.GetFace(face);

        o.submesh = mface.TintIndex == null ? 0 : 1;

        o.v1 = rot.MultiplyPoint(o.v1);
        o.v2 = rot.MultiplyPoint(o.v2);
        o.v3 = rot.MultiplyPoint(o.v3);
        o.v4 = rot.MultiplyPoint(o.v4);

        o.uv1 = uv1;
        o.uv2.y = uv1.y;
        o.uv2.x = uv2.x;
        o.uv3 = uv2;
        o.uv4.y = uv2.y;
        o.uv4.x = uv1.x;

        return o;
    }
}