using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Vec3I
{
	public int x, y, z;

    public override string ToString()
    {
        return ((Vector3Int)this).ToString();
    }

    public Vec3I(int x=0, int y=0, int z=0)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static implicit operator Vec3I(Vector3Int d) => new Vec3I(d.x,d.y,d.z);
	public static implicit operator Vector3Int(Vec3I d) => new Vector3Int(d.x, d.y, d.z);

}
