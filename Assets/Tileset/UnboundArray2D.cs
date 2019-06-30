using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnboundArray2D<T>
{
	public const int ChunkSize = 16;
	public class Chunk{
		public Vector2Int chunkCords;
		private T[,] Data = new T[ChunkSize,ChunkSize];
		public T this[Vector2Int index]
		{
			get { return Data.Index(index); }
			set { Data.Index(index, value); }
		}
	}

	Dictionary<Vector2Int, Chunk> chunkdict = new Dictionary<Vector2Int, Chunk>();
	public Chunk GetChunk(Vector2Int chunkCord) {
		Chunk o;
		if (chunkdict.TryGetValue(chunkCord, out o))
			return o;

		o = new Chunk();
		chunkdict[chunkCord] = o;
		return o;
	}

	public Vector2Int GetChunkCords(Vector2Int pos)
	{
		return pos.Div(ChunkSize);
	}
	
	public Vector2Int GetInChunkCords(Vector2Int pos)
	{
		return new Vector2Int( pos.x.ModPostive(ChunkSize), pos.y.ModPostive(ChunkSize));
	}

	public T this[Vector2Int pos]
	{
		get => GetChunk(GetChunkCords(pos))[GetInChunkCords(pos)];
		set => GetChunk(GetChunkCords(pos))[GetInChunkCords(pos)] = value;
	}
}
