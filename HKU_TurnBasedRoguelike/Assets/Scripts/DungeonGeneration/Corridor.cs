using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Corridor
{
    Room room;

    public enum Direction { north, east, south, west }
    public Direction direction;

    public Corridor(Direction _direction)
    {
        direction = _direction;
    }
}
