using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;

[System.Serializable]
public partial class UnboundArray3D<T>
{
	Dictionary<Vector3Int, Chunk> chunkdict = new Dictionary<Vector3Int, Chunk>();
	public Func<Vector3Int, Chunk> GetMissingChunk;

	public IEnumerable<Chunk> Chunks => chunkdict.Values;

	public Chunk GetChunk(Vector3Int chunkCord)
	{
		Chunk o;
		if (chunkdict.TryGetValue(chunkCord, out o))
			return o;
		
		o = GetMissingChunk?.Invoke(chunkCord) ?? new Chunk();
		chunkdict[chunkCord] = o;
		return o;
	}

	public Vector3Int GetChunkCords(Vector3Int pos)
	{
		return pos.Div(Chunk.Size);
	}

	public Vector3Int GetInChunkCords(Vector3Int pos)
	{
		return new Vector3Int(pos.x.ModPostive(Chunk.Size), pos.y.ModPostive(Chunk.Size), pos.z.ModPostive(Chunk.Size));
	}

	public T this[Vector3Int pos]
	{
		get => GetChunk(GetChunkCords(pos))[GetInChunkCords(pos)];
		set => GetChunk(GetChunkCords(pos))[GetInChunkCords(pos)] = value;
	}
}
