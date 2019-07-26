using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


struct BlockInstance
{
	Vector3Int pos;

}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRender : MonoBehaviour
{
	public UnboundArray3D<Tile>.Chunk chunk;
	private MeshFilter meshfilter;



	public void Awake()
	{
		meshfilter = GetComponent<MeshFilter>();
		Remesh();
	}
	static ChunkRender Make(UnboundArray3D<Tile>.Chunk chunk)
	{
		var go = new GameObject();
		go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		go.AddComponent<MeshCollider>();
		var cr = go.AddComponent<ChunkRender>();
		
		return cr;
	}


	public void LateUpdate()
	{
		if (chunk.IsDirty)
		{
			Remesh();
		}
	}
	// Start is called before the first frame update
	void Remesh()
	{
		var meshes = chunk
			.Where(U.Is)
			.Select(x => new CombineInstance() {
				mesh = x.Mesh,
				transform = x.transform.worldToLocalMatrix,

			}).ToArray();
		var mesh = new Mesh();
		mesh.CombineMeshes(combine: meshes);

		
		meshfilter.mesh = mesh;



		chunk.IsDirty = false;
	}
}
