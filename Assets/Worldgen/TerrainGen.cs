using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public bool DoUpdate = false;
    public Terrain terrain;
    public float amp = 1;
    public int octaves = 3;

    public float scale = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(DoUpdate)
        {
            terrain = GetComponent<Terrain>();

            DoUpdate = false;


            var data = terrain.terrainData;

            var heights = data.GetHeights(0, 0, data.heightmapWidth, data.heightmapHeight);

            var rng = new SimplexNoiseGenerator();
            var proni = new PRonii();

            double acc = 0;

            for (int y = 0; y < data.heightmapHeight; y++)
            {
                for (int x = 0; x < data.heightmapWidth; x++)
                {
                    var cords = scale*new Vector2(x, y);

                    float noise;
                    noise = rng.coherentNoise(x* scale, 0, y* scale, octaves);
                    //noise = Vector2.Distance(cords, proni.GetNearest(cords));
                    //noise = noise.Pow2();
                    //noise = Mathf.Min(noise, 0.3f);
                    noise = Mathf.Max(noise, .2f);


                    //noise = proni.g(cords).x;
                    //noise = Mathf.PerlinNoise(x* scale, y* scale);

                    acc += noise;
                    heights[x, y] = noise;
                }
            }

            acc /= data.heightmapHeight * data.heightmapWidth;
            print($"Mean {acc}");
            data.SetHeights(0, 0, heights);
            data.SyncHeightmap();
            data.RefreshPrototypes();
        }
        
    }
}
