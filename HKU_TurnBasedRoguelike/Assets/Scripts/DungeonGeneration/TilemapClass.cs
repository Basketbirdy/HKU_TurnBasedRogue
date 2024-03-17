using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TilemapClass
{
    public string name;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public int width;
    public int height;
}
