using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileUtils
{
    public static Vector3Int GetCellPosition(Tilemap tileMap,Vector3 worldPos)
    {
        Vector3Int currentTile = tileMap.WorldToCell(worldPos);
        return currentTile;
    }

    public static Vector3 GetWorldPosition(Tilemap tileMap, Vector3Int cellPos)
    {
        Vector3 currentTile = tileMap.CellToWorld(cellPos);
        return currentTile;
    }    
    
    public static Vector3 GetWorldCellPosition(Tilemap tileMap, Vector3Int cellPos)
    {
        Vector3 currentTile = tileMap.CellToWorld(cellPos);
        currentTile.x = currentTile.x + tileMap.cellSize.x / 2;
        currentTile.y = currentTile.y + tileMap.cellSize.y / 2;
        return currentTile;
    }
}
