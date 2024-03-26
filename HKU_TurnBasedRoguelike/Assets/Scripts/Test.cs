using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This was a test script for finding an issue with overlappoint collision

public class Test : MonoBehaviour
{
    public Tilemap tilemap;

    public LayerMask colliderLayerMask;
    public void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 worldposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldposition.z = 0;
            Vector3Int cellPosition = tilemap.WorldToCell(worldposition);
            Vector2 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);
            Collider2D collider = Physics2D.OverlapPoint(cellCenterWorld, colliderLayerMask);
            if(collider != null)
            {
                Debug.Log("ah");
            }

        }
    }
}
