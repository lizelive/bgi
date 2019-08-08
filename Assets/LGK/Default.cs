using Mc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class Default : MonoBehaviour
{
    public static ShitsOnFireYo YoOnFire => I.yoOnFire;

    public GameObject TargetIcon;
    public ShitsOnFireYo yoOnFire;
    public Controls controls;


    public Mesh buildingBlockMesh;
    public string playerName;
    public string AssetDir => Path.Combine(Application.persistentDataPath, "resourcepacks");

    public string respackToLoad = "John Smith Legacy 1.14.4 v8";

    public Material worldAtlas;

    public static Default I;
    public Rect[] rects;

    public McRespack respack;

    public List<Mesh> models = new List<Mesh>();

    public Texture2D atlasTexture;

    public Default()
    {
        I = this;
    }

    public void Start()
    {
        respack = new McRespack();
        respack.Load(Path.Combine(AssetDir, respackToLoad + ".zip"));
        rects = respack.textures.Values.ToArray();
        atlasTexture = respack.atlas;
        models = respack.meshes; //respack.models.Where(x => x.elements != null && x.elements.Any()).Take(10).ToList();
        worldAtlas.mainTexture = atlasTexture;

        File.WriteAllBytes("atlas.png", atlasTexture.EncodeToPNG());


    }

    public int modelSelect = 0;
    public void Update()
    {

        GetComponent<MeshFilter>().sharedMesh = models[modelSelect];
    }
}
