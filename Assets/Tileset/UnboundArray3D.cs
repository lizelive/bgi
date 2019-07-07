using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnboundArray3D<T>
{
	public const int ChunkSize = 16;
	public class Chunk
	{
		public Vector3Int chunkCords;
		private T[,,] Data = new T[ChunkSize, ChunkSize, ChunkSize];
		public T this[Vector3Int index]
		{
			get { return Data.Index(index); }
			set { Data.Index(index, value); }
		}
	}

	Dictionary<Vector3Int, Chunk> chunkdict = new Dictionary<Vector3Int, Chunk>();
	public Chunk GetChunk(Vector3Int chunkCord)
	{
		Chunk o;
		if (chunkdict.TryGetValue(chunkCord, out o))
			return o;

		o = new Chunk();
		chunkdict[chunkCord] = o;
		return o;
	}

	public Vector3Int GetChunkCords(Vector3Int pos)
	{
		return pos.Div(ChunkSize);
	}

	public Vector3Int GetInChunkCords(Vector3Int pos)
	{
		return new Vector3Int(pos.x.ModPostive(ChunkSize), pos.y.ModPostive(ChunkSize), pos.z.ModPostive(ChunkSize));
	}

	public T this[Vector3Int pos]
	{
		get => GetChunk(GetChunkCords(pos))[GetInChunkCords(pos)];
		set => GetChunk(GetChunkCords(pos))[GetInChunkCords(pos)] = value;
	}
}
