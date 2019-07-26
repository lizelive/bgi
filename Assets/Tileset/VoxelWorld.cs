using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
public class VoxelWorld : MonoBehaviour
{
	public Dictionary<Vector3Int, ChunkRender> renders = new Dictionary<Vector3Int, ChunkRender>();
	public bool doRender = true;
	public float viewDistance = 100;
	public float blockSize = 2;


	public Tile this[Vector3Int index]
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
		renders.Remove(cords);
	}


	UnboundArray3D<Tile> backing = new UnboundArray3D<Tile>();

	UnboundArray3D<Tile>.Chunk Load(Vector3Int cord){

        var filePath = GetChunkPath(cord);

        UnboundArray3D<Tile>.Chunk chunk = null;

        if (File.Exists(filePath))
		    using (var file = File.OpenRead(GetChunkPath(cord)))
		    {
                chunk = formatter.Deserialize(file) as UnboundArray3D<Tile>.Chunk;
		    }
        else
        {
            chunk = new UnboundArray3D<Tile>.Chunk();
        }

        chunk.cord = cord;
        var cr = ChunkRender.Make(chunk);

        renders[cord] = cr;
        cr.transform.parent = transform;
        
        return chunk;
    }

	string GetChunkPath(Vector3Int cord) => Path.Combine(
Application.persistentDataPath,
$"{cord.x}_{cord.y}_{cord.z}.bgc");

	void SaveChunk(UnboundArray3D<Tile>.Chunk chunk)
	{
		print("save to " + GetChunkPath(chunk.cord));
		using (var file = File.OpenWrite(GetChunkPath(chunk.cord)))
			formatter.Serialize(file, chunk);


	}


    private void Awake()
    {

        backing.GetMissingChunk = Load;
    }
    // Update is called once per frame
    void Update()
    {
		if(Input.GetKeyDown(KeyCode.F3))
		foreach (var chunk in backing.Chunks)
		{
			//TODO don't d
			Unload(chunk.cord);
		}

	}
}
