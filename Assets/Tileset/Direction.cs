using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Down,
    Up,
    North,
    South,
    West,
    East
}


public static class DirectionUtils
{

    public static Direction[] Directions = System.Enum.GetValues(typeof(Direction)) as Direction[];

    public static Direction FromVector(Vector3 vec)
    {
        return Directions.MinBy(x => Vector3.Angle(x.GetDirectionVec(), vec));
    }


    public static Vector3Int GetDirectionVec(this Direction dir)
    {
        switch (dir)
        {
            case Direction.Down:
                return new Vector3Int(0,-1,0);
            case Direction.Up:
                return new Vector3Int(0, 1, 0);
            case Direction.North:
                return new Vector3Int(0, 0, -1);
            case Direction.South:
                return new Vector3Int(0, 0, 1);
            case Direction.West:
                return new Vector3Int(-1, 0, 0);
            case Direction.East:
                return new Vector3Int(1, 0, 0);



            default:
                break;
        }

        return Vector3Int.zero;
    }
}