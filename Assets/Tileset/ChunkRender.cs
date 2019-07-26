using System.Collections.Generic;
using System.Linq;
using UnityEngine;


struct BlockInstance
{
    Vector3Int pos;

}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRender : MonoBehaviour
{
    private UnboundArray3D<Tile>.Chunk chunk;

    private MeshFilter meshfilter;
    private new MeshCollider collider;

    public void Awake()
    {
        meshfilter = GetComponent<MeshFilter>();
    }

    public static ChunkRender Make(UnboundArray3D<Tile>.Chunk chunk)
    {
        var go = new GameObject();
        go.AddComponent<MeshFilter>();
        var render =go.AddComponent<MeshRenderer>();

        render.sharedMaterial = Default.I.worldAtlas;

        

        go.name = $"ChunkRender {(Vector3Int)chunk.cord}";
        var cr = go.AddComponent<ChunkRender>();
        cr.chunk = chunk;
        cr.collider = go.AddComponent<MeshCollider>();


        go.transform.position = ((Vector3Int)chunk.cord) * UnboundArray3D<Tile>.Chunk.Size;

        return cr;
    }


    public void LateUpdate()
    {
        if (chunk?.IsDirty ?? false)
        {
            Remesh();
        }
    }


    IEnumerable<CombineInstance> GetStuffToRender()
    {
        //if (chunk == null)
        //{
        //    print("How");
        //}
        //else
        foreach (var innerPos in chunk.insideBounds.allPositionsWithin)
        {
            var tile = chunk[innerPos];
            if (tile.IsAir)
                continue;

            var neighbors = VecU.CardnalVec3I.Select(o => o + innerPos).ToArray();

            var edge = !neighbors.All(chunk.insideBounds.Contains);

            var hasAirNeighbor = neighbors.Any(p => chunk[p].IsAir);

            var shouldShow = !tile.IsAir && (edge || hasAirNeighbor);


            if (shouldShow || true)
                yield return new CombineInstance()
                {
                    mesh = tile.Mesh,
                    transform = Matrix4x4.Translate(innerPos)
                };

        }

    }
    // Start is called before the first frame update
    void Remesh()
    {
        var mesh = new Mesh();
        mesh.name = $"Voxel {chunk.cord} {Time.frameCount}";
        var todraw = GetStuffToRender().ToArray();
        mesh.CombineMeshes(combine: todraw);
        mesh.UploadMeshData(true); // not sure if this is the right call. we have so much to lose.
        meshfilter.sharedMesh = mesh;
        collider.sharedMesh = mesh;
        chunk.IsDirty = false;
    }
}
