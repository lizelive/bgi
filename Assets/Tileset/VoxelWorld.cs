using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Chunk = UnboundArray3D<BlockState>.Chunk;
using CompressionLevel = System.IO.Compression.CompressionLevel;
using UArray = UnboundArray3D<BlockState>;

public class VoxelWorld : MonoBehaviour
{
    public static VoxelWorld I; 
    // i hate unity
    public Dictionary<string, int> blockIds;

    public Block[] blocks;


    public Dictionary<Vector3Int, ChunkRender> renders = new Dictionary<Vector3Int, ChunkRender>();
    public bool doRender = true;
    public float viewDistance = 100;
    public float blockSize = 2;
    const CompressionLevel FileCompressionLevel = CompressionLevel.Optimal;

    public WorldGen worldGen;

    public BlockState this[Vector3Int index]
    {
        get { return backing[index]; }
        set { backing[index] = value; }
    }
    BinaryFormatter formatter = new BinaryFormatter();

    public void Unload(Vector3Int cords)
    {
        var chunk = backing.GetChunk(cords);

        SaveChunk(chunk);
        Destroy(renders[cords].gameObject);

        backing.RemoveChunk(cords);
        renders.Remove(cords);
    }


    UnboundArray3D<BlockState> backing = new UnboundArray3D<BlockState>();

    public VoxelWorld()
    {
        I = this;
    }

    Chunk Load(Vector3Int cord)
    {

        var filePath = GetChunkPath(cord);

        Chunk chunk = null;

        if (File.Exists(filePath))
            using (var file = new GZipStream(File.OpenRead(GetChunkPath(cord)), CompressionMode.Decompress))
            {
                chunk = formatter.Deserialize(file) as Chunk;

            }
        else
        {
            chunk = new Chunk();
            chunk.cord = cord;
            worldGen.Populate(chunk);
        }


        chunk.IsDirty = true;
        chunk.cord = cord;
        var cr = ChunkRender.Make(chunk);

        renders[cord] = cr;
        cr.transform.parent = transform;

        return chunk;
    }

    string GetChunkPath(Vector3Int cord) => Path.Combine(
Application.persistentDataPath,
$"{cord.x}_{cord.y}_{cord.z}.bgc");

    void SaveChunk(Chunk chunk)
    {
        print("save to " + GetChunkPath(chunk.cord));
        using (var file = new GZipStream(File.OpenWrite(GetChunkPath(chunk.cord)), FileCompressionLevel))
            formatter.Serialize(file, chunk);


    }

    public TextAsset blocksJson;

    private void Awake()
    {
        blocks = Newtonsoft.Json.JsonConvert.DeserializeObject<Block[]>(blocksJson.text);

        foreach (var block in blocks)
        {
            block.mesh = Default.I.buildingBlockMesh;
        }


        backing.GetMissingChunk = Load;
    }
    // Update is called once per frame
    void Update()
    {
        var meow = UArray.GetChunkCords(Vector3Int.FloorToInt(Camera.main.pos()));

        var bounds = new BoundsInt(meow - Vector3Int.one, Vector3Int.one * 3);

        foreach (var item in bounds.allPositionsWithin)
        {
            backing.GetChunk(item);
        }
        
        //if (Input.GetKeyDown(KeyCode.F3))
        foreach (var chunk in backing.Chunks.ToArray())
        {
            if (Camera.main.Distance(chunk.WorldPos) > viewDistance)
                Unload(chunk.cord);
        }

    }


    private void OnDestroy()
    {
        // don't you ever just destroy the world?
        foreach (var chunk in backing.Chunks.ToArray())
            Unload(chunk.cord);
    }
}
