using UnityEngine;
using Chunk = UnboundArray3D<BlockState>.Chunk;

public class WorldGen : MonoBehaviour
{


    float OctaveNoise2d(Vector2 pos, float scale, float decay, int levels)
    {
        float acc = 0;
        for (int i = 0; i < levels; i++)
        {
            var p = pos * scale * Mathf.Pow(1 / 2, i);
            var lmscale = Mathf.Pow(decay, i);
            acc += lmscale * (Mathf.PerlinNoise(p.x, p.y)*2-1);


        }
        return acc;
    }
    public float height = 10;
    public float scale = 10;
    public float decay = 0.5f;
    public int levels = 4;

    public void Populate(Chunk chunk)
    {

        // if i want this to be fast use cuda.
        // tldr screw performance

        var heights = new float[Chunk.Size, Chunk.Size];

        for (int x = 0; x < Chunk.Size; x++)
        {
            for (int z = 0; z < Chunk.Size; z++)
            {
                var p = new Vector2Int(x, z);
                var p2 = p + chunk.WorldPos.xz();
                var p3 = scale * (Vector2)p2;
                heights[x, z] = height * OctaveNoise2d(p3, scale, decay, levels);
            }
        }


        foreach (var pos in chunk.insideBounds.allPositionsWithin)
        {
            var worldPos = pos + chunk.WorldPos;
            //var mapHeight = height * OctaveNoise2d(worldPos.xz(), scale, decay, levels);
            var mapHeight = heights.Index(pos.xz());

            if(worldPos.y < mapHeight)
            chunk[pos] = new BlockState { blockId = 1 };


        }
    }
}
