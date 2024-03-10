using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float movementSpeed;
    [SerializeField] int movementRange = 2;

    [SerializeField] LayerMask obstaclesMask;

    [Header("References")]
    [SerializeField] Tilemap tilemap;
    [Header("Player")]
    [SerializeField] GameObject spriteObj;
    [Header("Scripts")]
    [SerializeField] PlayerCombat playerCombat;
    [Header("UI")]
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] SpriteRenderer indicatorRenderer;
    [SerializeField] Color positiveIndicator;
    [SerializeField] Color negativeIndicator;
    [Space]
    [SerializeField] ParticleSystem trailParticles;


    [Header("Debug")]
    [SerializeField] Vector3Int currentTile;
    public Vector3 targetPos;

    [SerializeField] int turnIndex;

    [Header("States")]
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isTurn = false;
    [SerializeField] public bool canMove = true;

    //[Header("Events")]
    public static event Action crossedScreenThreshold;

    // Start is called before the first frame update
    void Start()
    {
        TurnManager.instance.AddToList(gameObject);
        turnIndex = TurnManager.instance.ReturnObjIndex(gameObject);

        SnapToGrid(this.gameObject, 0, 0);
        currentTile = Vector3Int.zero;
        targetPos = transform.position;

        CheckTurn();
    }


    // Update is called once per frame
    void Update()
    {
        GetInput();

        // move player to target position
        transform.position = Vector2.MoveTowards(transform.position, targetPos, movementSpeed * Time.deltaTime);

        if(isMoving)
        {
            if(transform.position == targetPos)
            {
                isMoving = false;
                ParticleUtils.TriggerSystem(trailParticles, false);
                TurnManager.instance.AdvanceTurn(1);
            }
        }
    }

    private void OnEnable()
    {
        TurnManager.onAdvanceTurn += CheckTurn;
    }

    private void OnDisable()
    {
        TurnManager.onAdvanceTurn -= CheckTurn;   
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(targetPos, Vector3.one);
    }

    private void SnapToGrid(GameObject obj,  int x, int y)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(new Vector3Int(x,y,0));
        obj.transform.position = worldPos;
    }

    private void GetInput()
    {
        // get mouse position
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int closestCellToClick = TileUtils.GetCellPosition(tilemap, clickPos);

        selectionIndicator.transform.position = TileUtils.GetWorldCellPosition(tilemap, closestCellToClick);

        if((!canMove || !isTurn) && selectionIndicator.activeSelf == true) { selectionIndicator.SetActive(false); }
        else if((canMove && isTurn) && selectionIndicator.activeSelf == false) { selectionIndicator.SetActive(true); }

        if(MoveCheck(closestCellToClick, movementRange, false))
        {
            indicatorRenderer.color = positiveIndicator;
        }
        else
        {
            indicatorRenderer.color = negativeIndicator;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && canMove && isTurn)
        {
            // get clicked cell
            Debug.Log("Clicked at: " + clickPos);


            // check if the cell is within range
            if (MoveCheck(closestCellToClick, movementRange, true))
            {
                MoveToCell(closestCellToClick);
                isMoving = true;
            }
            else
            {
                Debug.LogWarning("Clicked position is outside of movement range or has an obstacle on it");
            }

            // flip the sprite if moving to the right
            if (targetPos.x > transform.position.x)
            {
                spriteObj.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (targetPos.x < transform.position.x)
            {
                spriteObj.GetComponent<SpriteRenderer>().flipX = false;
            }
            else { }
        }
    }

    public bool MoveCheck(Vector3Int clickedCell, int range, bool shouldAttack)
    {
        // clickedCell is the selected cell
        // checkPos is the cell whos range is being checked

        bool state = false;

        for (var i = -range; i <= range; i++)
        {
            for (var j = -range; j <= range; j++)
            {
                Vector3Int checkPos = new Vector3Int(currentTile.x + i, currentTile.y + j, 0); // get the cell that is being checked on if it is the selected cell
                if (clickedCell == checkPos)
                {
                    state = true;
                }
            }
        }

        //check if there are enemies or objects on the tile
        Collider2D[] obstacles =  Physics2D.OverlapBoxAll(new Vector2(clickedCell.x + tilemap.cellSize.x / 2, clickedCell.y + tilemap.cellSize.y / 2), Vector2.one * 0.1f, 0, obstaclesMask);
        //Debug.Log(obstacles.Length);

        // if there are obstacles
        if(obstacles.Length != 0) 
        { 
            state = false;

            if (!shouldAttack) { return state; }
            foreach (Collider2D obstacle in obstacles) 
            { 
                // if the obstacle isn't an enemy
                if(obstacle.gameObject.layer != 7) 
                {
                    ICollectable collectable = obstacle?.GetComponent<ICollectable>();
                    collectable.Collect();
                    continue;
                }

                playerCombat.DealDamage(obstacle.gameObject);
            }
        }


        return state;
    }

    public void MoveToCell(Vector3Int targetTile)
    {
        isMoving = true;
        ParticleUtils.TriggerSystem(trailParticles, true);

        // get target position
        targetPos = TileUtils.GetWorldCellPosition(tilemap, targetTile);

        Debug.Log("Moved towards: " + targetPos);

        currentTile = targetTile;
    }

    private void CheckTurn()
    {
        int currentIndex = TurnManager.instance.ReturnActiveIndex();

        if(currentIndex == turnIndex)
        {
            isTurn = true;
        }
        else
        {
            isTurn= false;
        }
            
        Debug.Log("Turns; triggered CheckTurn function");
    }

    public Vector3Int GetCurrentCellPosition()
    {
        return currentTile;
    }
}
