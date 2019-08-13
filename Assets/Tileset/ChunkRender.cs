using System.Collections.Generic;
using System.Linq;
using UnityEngine;





[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRender : MonoBehaviour
{
    private UnboundArray3D<BlockState>.Chunk chunk;

    private MeshFilter meshfilter;
    private new MeshCollider collider;

    public void Awake()
    {
        meshfilter = GetComponent<MeshFilter>();
    }

    public static Material colored;

    public static ChunkRender Make(UnboundArray3D<BlockState>.Chunk chunk)
    {
        var go = new GameObject();
        go.AddComponent<MeshFilter>();
        var render =go.AddComponent<MeshRenderer>();
        var mc = go.AddComponent<MeshCollider>();
        var mat= Default.I.worldAtlas;

        var matColored = colored??new Material(mat);
        matColored.color = Color.green;
        render.sharedMaterials = new Material[] { mat, matColored };
        

        go.name = $"ChunkRender {(Vector3Int)chunk.cord}";
        var cr = go.AddComponent<ChunkRender>();
        cr.chunk = chunk;
        cr.collider = mc;



        //Vector3.up / 2
        go.transform.position = chunk.WorldPos;

        return cr;
    }


    public void LateUpdate()
    {
        if (chunk?.IsDirty ?? false)
        {
            Remesh();
        }
    }


    IEnumerable<CombineInstance> GetStuffToRender(int submesh = 0)
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

            var neighbors = VecU.PureDirs.Select(o => o + innerPos).ToArray();
            


            var shouldShow = !tile.IsAir && (
                !neighbors.All(chunk.insideBounds.Contains) || // is an edge
                neighbors.Any(p => chunk[p].IsAir) // has an neighbor that is air
                
                );       


            if (shouldShow)
            {
                var mesh = tile.Mesh;
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    if(i == submesh)
                    yield return new CombineInstance()
                    {
                        mesh = mesh,
                        transform = Matrix4x4.Translate(innerPos),
                        subMeshIndex = i
                    };
                }

            }

        }

    }

    
    void Remesh()
    {
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = $"Voxel {chunk.cord} {Time.frameCount}";

        var submeshes = Enumerable.Range(0, 2).Select(i =>
          {
              var m = new Mesh();
              m.CombineMeshes(GetStuffToRender(i).ToArray());
              

              return new CombineInstance
              {
                  mesh = m ?? new Mesh(),
                  transform = Matrix4x4.identity
              };
          }).ToArray();

        mesh.CombineMeshes(submeshes, false);
        mesh.subMeshCount = 2;

        

        mesh.UploadMeshData(true); // not sure if this is the right call. we have so much to lose.
        meshfilter.sharedMesh = mesh;
        collider.sharedMesh = mesh;
        chunk.IsDirty = false;
    }
}

