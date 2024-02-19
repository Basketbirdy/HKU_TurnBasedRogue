using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float movementSpeed;

    [Header("References")]
    [SerializeField] Tilemap tilemap;

    [Header("Debug")]
    [SerializeField] Vector3Int currentTile;

    // Start is called before the first frame update
    void Start()
    {
        SnapToGrid(this.gameObject, 0, 0);
        currentTile = Vector3Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        Vector2 direction;
        direction.x = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        direction.y = Mathf.Abs(Input.GetAxisRaw("Vertical"));

        //Debug.Log("Direction: " + direction.x + ", " + direction.y);

        if(direction == Vector2.zero) { }
        else if (direction.x == -1)
        {
            // Move to the left
        }
        else if (direction.x == 1)
        {
            // Move to the right
        }
        else if (direction.y == -1)
        {
            // Move down
        }
        else if (direction.y == 1)
        {
            // Move up
        }

    }

    private void SnapToGrid(GameObject obj,  int x, int y)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(new Vector3Int(x,y,0));
        obj.transform.position = worldPos;
    }

    //public Vector3Int GetTargetCell()
    //{
    //    Vector3Int targetCell;
        
    //    return targetCell;
    //}

    public void MoveToCell(int x, int y)
    {
        // get target tile
        Vector3Int targetTile = new Vector3Int(x,y,0);
        Vector3 targetPos = tilemap.CellToWorld(targetTile);

        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
    }
}
