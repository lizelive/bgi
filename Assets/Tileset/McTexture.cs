using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class McTexture
{
    const int AtlasSize = 4096;
    Rect uvs;
    string name;
    // some shit about animation or something;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Texture2D t = new Texture2D(AtlasSize, AtlasSize);
        //t.PackTextures()
        
    }


}
