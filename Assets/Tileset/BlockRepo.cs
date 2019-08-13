using System;
using System.Collections.Generic;
using System.Linq;

public class BlockRepo
{
    public static Dictionary<string, Block> blockById;
    public static Block[] blocks;

    static BlockRepo()
    {

        //TODO don't do this. please do literly anything else.
        var path = @"C:\Users\Lize\source\bgi\Assets\Tileset\blocks.json";
        var json = System.IO.File.ReadAllText(path);
        var freshBlocks = Newtonsoft.Json.JsonConvert.DeserializeObject<Block[]>(json);

        blocks = new Block[Math.Max(freshBlocks.Length, freshBlocks.Max(x => x.idnum)+1)];

        foreach (var block in freshBlocks)
        {
            if (block.idnum != 0)
            {
                blocks[block.idnum] = block;
            }
        }

        {
            ushort i = 0;
            foreach (var block in freshBlocks)
            {
                if (block.idnum != 0)
                    continue;
                while (blocks[i] != null)
                    i++;
                block.idnum = i;
                blocks[i] = block;
            }
        }




        for (ushort i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            if (block == null)
                continue;
            var props = block.props;
            if (props != null)
            {
                byte pos = 0;
                foreach (var prop in props)
                {
                    var size = prop.size = (byte)Math.Ceiling(Math.Log(prop.values.Length, 2));
                    prop.position = pos;
                    pos += size;
                }
            }


        }

        blockById = blocks.Where(U.Is).ToDictionary(x => x.id, x => x);

    }

    public static Block Get(string id)
    {
        if (blockById.TryGetValue(id, out var block))
        {
            return block;
        }
        return null;
    }

    public static Block Get(int id)
    {
        if (id < blocks.Length)
            return blocks[id];
        return null;
    }
}
