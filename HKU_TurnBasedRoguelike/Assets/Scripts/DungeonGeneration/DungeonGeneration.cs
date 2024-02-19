using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Vector2Int worldGridRadius = new Vector2Int(3,4);

    // cool MATH numbers for cooler randomness
    [Header("Magic numbers")]
    [SerializeField] float randomCompareStart = 0.2f;
    [SerializeField] float randomCompareEnd = 0.01f;

    [Header("Data")]
    [Header("Rooms")]
    [SerializeField] Room[,] rooms;
    [SerializeField] int roomAmount = 12; 

    [Header("Room data")]
    [SerializeField] List<Vector2Int> takenPositions;
    int gridSizeX, gridSizeY;

    [Header("Debug")]
    [SerializeField] GameObject debugRoom;

    // Start is called before the first frame update
    void Start()
    {
        // check wether or not there are more rooms than fit in the given grid
        if(roomAmount > (worldGridRadius.x * 2) * (worldGridRadius.y * 2))
        {
            roomAmount = (worldGridRadius.x * 2) * (worldGridRadius.y * 2);
            Debug.LogWarning("Amount of rooms specified don't fit in the specified grid");
        }

        gridSizeX = worldGridRadius.x;
        gridSizeY = worldGridRadius.y;

        CreateRooms(); // Create a grid filled with rooms

        Debug.Log(rooms.Length);

        SetRoomDoors(); // set the door states of each room

        DrawMap(); // draw the dungeon
    }

    private void CreateRooms()
    {
        rooms = new Room[worldGridRadius.x * 2, worldGridRadius.y * 2]; // Create room array according to grid size

        rooms[gridSizeX, gridSizeY] = new Room(Vector2Int.zero, Room.RoomType.start, 20, 10); // create room at the center of the grid
        takenPositions.Insert(0, Vector2Int.zero); // add room to occupancy list
        Vector2Int checkPos = Vector2Int.zero; // create room to use as reference position while determining next room

        // create remaining rooms
        // Magic numbers
        float randomCompare = 0.2f;
        // add rooms
        for (int i = 0; i < roomAmount - 1; i++)
        {
                float randomPerc = ((float) i / (roomAmount - 1)); // Percentage of rooms that still need to be spawned
                randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc); // The more rooms already determined the less rooms branch out

            checkPos = NewPosition();

            // Check for available positions
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && UnityEngine.Random.value > randomCompare) // <--------- error in here !!!
            {
                int attempts = 0;
                do
                {
                    checkPos = SelectiveNewPosition();
                    attempts++;
                }while (NumberOfNeighbors(checkPos, takenPositions) > 1 && attempts < 100);
                if(attempts >= 50) 
                {
                    Debug.LogWarning("Could not create a room with fewer neighbors than: " + NumberOfNeighbors(checkPos, takenPositions));
                }
            }

            // finalize position of the room
            rooms[checkPos.x + gridSizeX, checkPos.y + gridSizeY] = new Room(checkPos, Room.RoomType.empty, 20, 10);
            Debug.Log("Checkpos: " + checkPos);
            takenPositions.Insert(0,checkPos);
        }
    }

    private Vector2Int NewPosition()
    {
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        do
        {
            // get a random occupied position
            int index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1)); // get random index from the list (random position in the grid)
            Debug.Log("Index: " + index);
            x = takenPositions[index].x; // set position from index
            y = takenPositions[index].y; // set position from index

            bool UpOrDown = (UnityEngine.Random.value < 0.5f); // Determine if you check vertically or horizontally from current room
            bool direction = (UnityEngine.Random.value < 0.5f); // Determine if you check before or after the current room in the determined orientation
            // alter positions
            if (UpOrDown)
            {
                if (direction) { y += 1; }
                else { y -= 1; }
            }
            else
            {
                if (direction) { x += 1; }
                else { x -= 1; }
            }
            checkingPos = new Vector2Int(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
        return checkingPos;
    }

    private Vector2Int SelectiveNewPosition()
    {
        int index = 0, increment = 0;
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        do
        {
            increment = 0;
            do
            {
                // get a random occupied position
                index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1)); // get random index from the list (random position in the grid)
                Debug.Log("Index: " + index);
                increment++;
            }
            while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && increment < 100);

            x = takenPositions[index].x; // set position from index
            y = takenPositions[index].y; // set position from index

            bool UpOrDown = (UnityEngine.Random.value < 0.5f); // Determine if you check vertically or horizontally from current room
            bool direction = (UnityEngine.Random.value < 0.5f); // Determine if you check before or after the current room in the determined orientation
            // alter positions
            if (UpOrDown)
            {
                if (direction) { y += 1; }
                else { y -= 1; }
            }
            else
            {
                if (direction) { x += 1; }
                else { x -= 1; }
            }
            checkingPos = new Vector2Int(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);

        if(increment >= 100)
        {
            Debug.LogWarning("Could not find position with only one neighbor");
        }

        return checkingPos;
    }

    private int NumberOfNeighbors(Vector2Int checkingPos, List<Vector2Int> takenPositions)
    {
        int sum = 0;
        
        if(takenPositions.Contains(checkingPos + Vector2Int.right)) { sum++; }
        if(takenPositions.Contains(checkingPos + Vector2Int.left)) { sum++; }
        if(takenPositions.Contains(checkingPos + Vector2Int.up)) { sum++; }
        if(takenPositions.Contains(checkingPos + Vector2Int.down)) { sum++; }

        Debug.Log("Amount of neighbors: " +  sum);
        return sum;
    }

    void SetRoomDoors()
    {
        for (int x = 0; x < (gridSizeX*2); x++)
        {
            for(int y = 0; y < (gridSizeY*2); y++) // loop through all the possible grid positions
            {
                if (rooms[x, y] == null)
                {
                    continue; // continue with the  next check of the loop 
                }

                Vector2Int gridPosition = new Vector2Int(x, y); // create gridPosition

                // check if there is a room in all directions from current room 
                if(y - 1 < 0) { rooms[x, y].south = false; } // check north
                else { rooms[x, y].north = rooms[x,y - 1] != null; }

                if(y + 1 >= gridSizeY * 2) { rooms[x, y].north = false; } // check south
                else { rooms[x,y].north = rooms[x, y + 1] != null; }

                if (x - 1 < 0) { rooms[x, y].west = false; } // check west
                else { rooms[x, y].west = rooms[x - 1, y] != null; }

                if (x + 1 >= gridSizeY * 2) { rooms[x, y].east = false; } // check east
                else { rooms[x, y].east = rooms[x + 1, y] != null; }

                Debug.Log("x: " + x + "y: " + y);
            }
        }    
    }

    private void DrawMap()
    {
        foreach(Room room in rooms) 
        {
            if (room == null)
            {
                Debug.Log("room is null");
                continue;
            }

            // get grid positions 
            Vector3Int drawPos = new Vector3Int(room.gridPos.x, room.gridPos.y, 0);

            // get world positions
            drawPos.x *= room.width;
            drawPos.y *= room. height;

            // Spawn the room 
            Instantiate(debugRoom, drawPos, quaternion.identity); 

        }
    }
}
