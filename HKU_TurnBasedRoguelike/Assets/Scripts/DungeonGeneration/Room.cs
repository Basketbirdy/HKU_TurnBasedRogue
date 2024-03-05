using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    [Header("Description")]
    public string roomName;

    [Header("Room data")]
    public RoomType type;
    public enum RoomType { start, end, boss, item , empty, shop}

    public int width; // Width of the room
    public int height; // Height of the room

    public Vector2Int gridPos;

    public bool north, east, south, west;

    public Room(Vector2Int _gridPos, RoomType _type, int _width, int _height)
    {
        type = _type; // set the type of room

        width = _width; height = _height; // set the width and height for the room
        gridPos = _gridPos; // set the grid position of the room
    }
}
