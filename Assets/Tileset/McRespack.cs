using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
public class McRespack
{

    public string name, description;
    public Texture2D atlas;
    public const int AtlasSize = 4096;
    public int padding = 0;
    public Dictionary<string, Rect> textures;
    public List<Mc.McModel> models = new List<Mc.McModel>();

    public List<Mesh> meshes = new List<Mesh>();
    /// <summary>
    /// load a zip for archive
    /// </summary>
    /// <param name="path"></param>
    public void Load(string path)
    {
        using (var archive = ZipFile.Open(path, ZipArchiveMode.Read))
        {
            var textureFiles = archive.Entries.Where(e => e.Name.EndsWith(".png") && e.FullName.StartsWith("assets/minecraft/textures")).ToArray();

            
            var textures = textureFiles.Select(e =>
            {
                var t = new Texture2D(0, 0);
                t.name = e.FullName;
                var data = new byte[e.Length];
                using (var stream = e.Open())
                {
                    stream.Read(data, 0, data.Length);
                }
                t.LoadImage(data);
                return t;
            }).ToArray();
            atlas = new Texture2D(AtlasSize, AtlasSize);
            atlas.filterMode = FilterMode.Point;
            var rects = atlas.PackTextures(textures, padding);


            this.textures = textureFiles.Select(x => x.FullName.Substring(26,x.FullName.Length-26-4)).Zip(rects, (a, b) => (a, b)).ToDictionary(x => x.a, x => x.b);

            // block models
            var jsonFiles = archive.Entries.Where(e => e.FullName.EndsWith(".json")).ToArray();

            var blockstateFiles = archive.Entries.Where(e => e.FullName.Contains("assets\\minecraft\\blockstates") && e.FullName.EndsWith(".json")).ToArray();

            foreach (var block in blockstateFiles)
            {

            }


            var modelFiles = archive.Entries.Where(e => e.FullName.Contains("models") && e.FullName.EndsWith(".json")).ToArray();

            foreach (var modelFile in modelFiles)
            {
                using (var stream = new StreamReader(modelFile.Open()))
                {
                    var json = stream.ReadToEnd();
                    Mc.McModel model = null;

                    try
                    {

                    model = Newtonsoft.Json.JsonConvert.DeserializeObject<Mc.McModel>(json);
                        models.Add(model);
                    }
                    catch (System.ArgumentException e)
                    {
                        Debug.LogException(e);
                        Debug.LogError(json);
                    }


                    if (model.elements != null)
                    {
                        var quads = model.elements.SelectMany(e => DirectionUtils.Directions.Select(d => e.GetQuad(model, this, d))).Where(U.Is).ToArray();

                        var mesh = new Mesh();

                        var indices = new int[quads.Length * 4];
                        var verts = new Vector3[quads.Length * 4];
                        var uv = new Vector2[quads.Length * 4];

                        for (int i = 0; i < quads.Length; i++)
                        {
                            var b = i * 4;
                            var q = quads[i];

                            indices[b + 0] = b + 0;
                            indices[b + 1] = b + 1;
                            indices[b + 2] = b + 2;
                            indices[b + 3] = b + 3;

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

                        mesh.SetIndices(indices, MeshTopology.Quads, 0);
                        
                        mesh.name = modelFile.FullName;

                        mesh.RecalculateNormals();
                        mesh.RecalculateTangents();
                        mesh.RecalculateBounds();
                        mesh.UploadMeshData(false);
                        
                        meshes.Add(mesh);
                    }
              

                }
            }

        }
    }

}

public class Quad
{
    public Vector3 v1, v2, v3, v4;
    public Vector2 uv1, uv2, uv3, uv4;
}

public static class McModelExtensions
{

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
        var from = element.from.ToVec3()/16+offset;
        var to = element.to.ToVec3()/16+offset;

        var mface = element.faces.GetFace(face);

        if (mface==null|| mface.Uv==null) return null;




        var texture = new Rect(0, 0, 1, 1);



        var texturePath = mface.Texture;


        model.textures?.TryGetValue(mface.Texture.TrimStart('#'), out texturePath);

        texturePath = texturePath ?? "";

        if (pack.textures.TryGetValue(texturePath, out texture)){

        }
        else
        {
            //Debug.LogWarning($"Missing texture {mface.Texture}");
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

        o.uv1 = uv1;
        o.uv2.y = uv1.y;
        o.uv2.x = uv2.x;
        o.uv3 = uv2;
        o.uv4.y = uv2.y;
        o.uv4.x = uv1.x;

        return o;
    }
}