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


        public Vector3Int WorldPos => (Vector3Int)cord * Size;

        public override string ToString()
        {
            return $"Chunk {cord}";
        }
        public BoundsInt insideBounds => new BoundsInt(0, 0, 0, Size, Size, Size);
        public BoundsInt worldBounds => new BoundsInt(cord.x, cord.y, cord.z, Size, Size, Size);

        public const int Size = 16;
		public Vec3I cord;
		private T[,,] Data = new T[Size, Size, Size];
		public T this[Vector3Int index]
		{
			get { return Data.Index(index);   }
			set { Data.Index(index, value); IsDirty = true; }
		}

        public bool IsDirty { get; set; } = true;

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

        void MakeNavGreatAgain()
        {

        }

	}
}
