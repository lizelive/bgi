using System;
using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public static TileGrid I;
    TileGrid()
    {
        I = this;
    }

    public BlockState Get(Vector3Int pos) => world[pos];

    public BlockState this[Vector3Int index]
    {
        get => world[index];

        set => world[index] = value;
    }

    //[SerializeField]
    public VoxelWorld world;

    public BlockState build = new BlockState { blockId = 1 };


    public float gridSize => world.blockSize;

    public Tuple<Vector2Int, Vector2Int>[] maze;


    void GenMaze()
    {
        var mazeSize = 10;
        var gridSize = 2 * mazeSize + 1;

        var build = new bool[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var even = x % 2 + y % 2 == 2;
                build[x, y] = !even;
            }
        }

        maze = new MazeGenerator().Generate().ToArray();
        foreach (var path in maze)
        {
            var gapPoint = path.Item1 + path.Item2;
            build.Index(gapPoint + Vector2Int.one, false);
        }


        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var pos = new Vector3Int(x, 0, y);
                pos.y = build.Index(pos.xz()) ? 1 : 0;
                Build(pos);
            }
        }
    }
    /*
	 * so game dev question. how far designed is your 
	*/
    // Update is called once per frame




    Vector3Int lastSelect, cell2d;


    public bool Build(Vector3Int pos)
    {
        var has = this[pos];
        if (has.IsAir)
        {
            //print($"Build {pos} {build}");
            this[pos] = build;
            return true;
        }
        return false;
    }

    public bool Break(Vector3Int cell)
    {
        var boi = this[cell];
        //print($"Break {cell} {boi}");
        if (!boi.IsAir)
        {

            this[cell] = BlockState.Air;
            return true;
        }
        return false;
    }

    public bool abort;
}


public partial class Player
{
    public UnityEngine.Transform buildPreview;
    private bool showPreview = false;

    public TileGrid grid;
    public bool showDebugInfo = true;
    public float buildCost = 1;
    private Vector3Int selectedCell;

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.Label($"{selectedCell}");
            GUILayout.Label($"{grid[selectedCell]}");
        }
    }

    private void LateUpdate()
    {
        DrawCurrentBlockUI();
    }

    void DrawCurrentBlockUI()
    {
        var mesh = grid.build.Mesh ?? Default.I.buildingBlockMesh;
        var cam = Camera.main;
        var pos = cam.ViewportToWorldPoint(new Vector3(.9f, .9f, 3));
        var drawmat = Matrix4x4.TRS(pos, cam.transform.rotation, Vector3.one/3);
        //drawmat = Matrix4x4.identity;
        var mat = Default.I.worldAtlas;
        Graphics.DrawMesh(mesh, drawmat, mat, 5);
    }

    void UpdateBuild()
    {

        showDebugInfo ^= Input.GetKeyDown(KeyCode.F3);
        //foreach (var move in maze)
        //{
        //	Debug.DrawLine(move.Item1.x0y(), move.Item2.x0y());
        //}

        var doSelect = InMan.Melee;
        var toggleView = InMan.BuildmodeMC;

        showPreview ^= toggleView;

        buildPreview.gameObject.SetActive(showPreview);

        var pos = targeter.position;
        selectedCell = (pos / grid.gridSize - 0.1f * targeter.up).RoundToInt();
        pos += grid.gridSize / 2 * targeter.up;
        var cell = (pos / grid.gridSize).RoundToInt();


        pos = grid.gridSize * (Vector3)cell;
        buildPreview.localScale = Vector3.one * grid.gridSize;
        buildPreview.position = grid.gridSize * (Vector3)selectedCell;
        var cell2d = cell;



        // slect logic


        //if (doSelect)
        //{
        //	var selected = new Region2D(lastSelect, cell2d);
        //	var validSelect = Room.IsValid(this, selected);
        //	print($"select was {validSelect}, {selected}");
        //	lastSelect = cell2d;
        //}

        //delete logic
        if (InMan.BreakMC)
        {
            if (grid.Break(selectedCell))
                Team.Balance += buildCost;
        }


        // build logic
        if (InMan.ChangeBlockUp)
        {
            grid.build = new BlockState { blockId = (ushort)((grid.build.blockId + 1) % Default.I.models.Count) };
        }
        else if (InMan.ChangeBlockDown)
        {
            grid.build = new BlockState { blockId = (ushort)((grid.build.blockId - 1).ModPostive(Default.I.models.Count)) };
        }

        if (InMan.BuildMC)
        {
            if (grid.Build(cell))
                Team.Balance -= buildCost;
        }

        

    }
}


















