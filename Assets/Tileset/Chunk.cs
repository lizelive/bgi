using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class UnboundArray3D<T>
{
	[System.Serializable]
	public class Chunk : IEnumerable<T>
	{
		public Vector3Int chunkCords;
		private T[,,] Data = new T[ChunkSize, ChunkSize, ChunkSize];
		public T this[Vector3Int index]
		{
			get { return Data.Index(index); }
			set { Data.Index(index, value); }
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in Data)
			{
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
