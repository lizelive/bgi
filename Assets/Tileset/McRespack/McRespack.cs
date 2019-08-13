using Mc;
using Newtonsoft.Json;
using System;
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
    public int padding = 4;
    public Dictionary<string, Rect> textures;
    public Dictionary<string, Mc.BlockStates> blockstates = new Dictionary<string, BlockStates>();
    public Dictionary<string, Mc.McModel> models = new Dictionary<string, Mc.McModel>();

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


            this.textures = textureFiles.Select(x => x.FullName.Substring(26, x.FullName.Length - 26 - 4)).Zip(rects, (a, b) => (a, b)).ToDictionary(x => x.a, x => x.b);

            // block models
            var jsonFiles = archive.Entries.Where(e => e.FullName.EndsWith(".json")).ToArray();

            var blockstateFiles = archive.Entries.Where(e => e.FullName.Contains("assets/minecraft/blockstates/") && e.FullName.EndsWith(".json")).ToArray();

            foreach (var blockstateFile in blockstateFiles)
            {

                var name = blockstateFile.Name.Substring(0, blockstateFile.Name.Length - 5);

                using (var stream = new StreamReader(blockstateFile.Open()))
                {
                    var json = stream.ReadToEnd().Trim(' ', '\n', '\r');

                    json = string.Join(" ", json.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));


                    BlockStates blockstate;
                    try
                    {
                        blockstate = JsonConvert.DeserializeObject<Mc.BlockStates>(json);

                    }
                    catch (JsonReaderException e)
                    {
                        //Debug.LogError($"{name} has bad json at {e.LineNumber} {e.LinePosition}");
                        json = json.Substring(0, e.LinePosition - 1);
                        blockstate = JsonConvert.DeserializeObject<Mc.BlockStates>(json);
                    }

                    blockstate.name = blockstate.name ?? name;
                    blockstates[name] = blockstate;
                }
            }

            var modelFiles = archive.Entries.Where(e => e.FullName.Contains("assets/minecraft/models/") && e.FullName.EndsWith(".json")).ToArray();

            foreach (var modelFile in modelFiles)
            {

                var prefixLength = "assets/minecraft/models/".Length;
                var name = modelFile.FullName.Substring(prefixLength, modelFile.FullName.Length - prefixLength - 4 - 1);
                using (var stream = new StreamReader(modelFile.Open()))
                {
                    var json = stream.ReadToEnd();
                    Mc.McModel model = null;

                    try
                    {

                        model = Newtonsoft.Json.JsonConvert.DeserializeObject<Mc.McModel>(json);



                        models.Add(name, model);
                    }
                    catch (System.ArgumentException e)
                    {
                        Debug.LogException(e);
                        Debug.LogError(json);
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                foreach (var model in models.Values)
                {
                    if (model.parent != null)
                    {
                        if (models.TryGetValue(model.parent, out var mom))
                        {
                            model.Parent(mom);
                            if (mom.textures != null && model.textures != null)
                                foreach (var att in mom.textures.Keys.Except(model.textures.Keys).ToArray())
                                {
                                    model.textures[att] = mom.textures[att];
                                }
                        }

                    }
                }
            }


            foreach (var model in models.Values)
            {
                if (model.elements != null)
                {
                    var mesh = model.GetMesh(this);
                    meshes.Add(mesh);
                }
            }


            foreach (var state in blockstates.Values)
            {
                var block = BlockRepo.Get(state.name);

                if (block == null)
                {
                    continue;
                }

                block.blockStates = state;
                block.display = GetDisplayMatrix(block);
                var meshes = new Mesh[256][];

                if (state.variants != null && state.variants.Any())
                {
                    foreach (var vars in state.variants)
                    {
                        var s = block.ParseState(vars.Key);

                        var magicNumber = s.magicNumber;
                        meshes[magicNumber] = vars.Value.value.Select(x => models[x.model].GetMesh(this)).Where(U.Is).ToArray();
                    }
                }
                else if (state.multipart != null)
                {
                    foreach (var mp in state.multipart)
                    {

                    }

                }
                block.render = meshes;

            }
        }
    }

    private Matrix4x4 GetDisplayMatrix(Block block)
    {
        return Matrix4x4.identity;
        // TODO : Craig finish function and remeber block.blockStates is nullable

        //empty case - I don't know how you're setting the defaults and I'm agnostic of it. Yaaaaay!
        if ((block.blockStates.variants == null && block.blockStates.variants.Count == 0) &&
            (block.blockStates.multipart == null && block.blockStates.multipart.Length == 0))
        {
            return Matrix4x4.identity;
        }
        //multipart case
        if (block.blockStates.variants == null || block.blockStates.variants.Count == 0)
        {
            return Matrix4x4.identity;
        }
        //standard variant case
        else if (block.blockStates.variants.TryGetValue("", out var model))
        {
            return Matrix4x4.identity;
        }
        //pick-a-variant case
        else
        {
            return Matrix4x4.identity;
        }
    }

}
