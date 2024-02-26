using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    [Header("Booleans")]
    bool isTraversable = true;

    [Header("Buffs & Debuffs")]
    int defenseBoost = 1;
}
