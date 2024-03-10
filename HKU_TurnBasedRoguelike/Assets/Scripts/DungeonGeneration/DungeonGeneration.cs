using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField] int roomwidth;
    [SerializeField] int roomheight;
    [SerializeField] int cheeseAmount;
    [SerializeField] int enemyAmount;

    [Header("Corridors")]
    [Header("Corridor data")]
    [SerializeField] int horCorridorWidth = 3;
    [SerializeField] int horCorridorHeight = 4;
    [SerializeField] int verCorridorWidth = 4;
    [SerializeField] int verCorridorHeight = 3;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap tilemap;
    [Header("Prefabs")]
    [SerializeField] private TilemapClass[] tileMapPrefabs;
    [SerializeField] private GameObject cheeseCollectable;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Debug")]
    [SerializeField] GameObject debugRoom;
    [SerializeField] bool spawnDebugCorridors;

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

        SpawnFinalChamber();

        SpawnShopChamber();

        SetRoomDoors(); // set the door states of each room

        DrawMap(); // draw the dungeon

        CreateCorridors();

        // spawn obstacles and cheese
        CreateObjects(cheeseCollectable, cheeseAmount);
        CreateObjects(enemyPrefab, enemyAmount);
    }

    private void CreateObjects(GameObject prefab, int amount)
    {
        // for the amount of cheese to spawn
        for(int i = 0; i < amount; i++)
        {
            // get random room to spawn object in
            Vector2Int roomPos = takenPositions[Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1))];

            // get world position of the room
            roomPos.x *= roomwidth + 1;
            roomPos.y *= roomheight + 1;

            // generate random offset within room bounds
            Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-(roomwidth / 2 - 2), roomwidth / 2 - 1),
                                               UnityEngine.Random.Range(-(roomheight / 2 - 2), roomheight / 2 - 1));

            // get world position
            Vector2 spawnPos = roomPos + randomOffset;

            // convert world position to cell position
            Vector3Int spawnCellPos = TileUtils.GetCellPosition(tilemap, spawnPos);
            //convert cell position into worldposition
            Vector3 finalSpawnPos = TileUtils.GetWorldCellPosition(tilemap, spawnCellPos);

            // spawn cheese
            Instantiate(prefab, finalSpawnPos, quaternion.identity);
        }
    }

    private void CreateRooms()
    {
        rooms = new Room[(worldGridRadius.x * 2) + 2, (worldGridRadius.y * 2) + 2]; // Create room array according to grid size

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
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && UnityEngine.Random.value > randomCompare) // <--------- error in here !!! ( FIXED :D:D:D:D:D )
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
            takenPositions.Insert(0,checkPos);

            //Debug.Log("Rooms_Doors: North: " + rooms[checkPos.x + gridSizeX, checkPos.y + gridSizeY].north);
            //Debug.Log("Rooms_Doors: East: " + rooms[checkPos.x + gridSizeX, checkPos.y + gridSizeY].east);
            //Debug.Log("Rooms_Doors: South: " + rooms[checkPos.x + gridSizeX, checkPos.y + gridSizeY].south);
            //Debug.Log("Rooms_Doors: West: " + rooms[checkPos.x + gridSizeX, checkPos.y + gridSizeY].west);
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
            //Debug.Log("Index: " + index);
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
                //Debug.Log("Index: " + index);
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

        //Debug.Log("Amount of neighbors: " +  sum);
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

                //Debug.Log("Doors; x: " + x + ", y: " + y);
                //Debug.Log("Doors; N: " + rooms[x, y].north + ", E: " + rooms[x,y].east + ", S: " + rooms[x,y].south + ", W: " + rooms[x,y].west);
            }
        }    
    }

    private void DrawMap()
    {
        foreach(Room room in rooms) 
        {
            if (room == null)
            {
                //Debug.Log("room is null");
                continue;
            }

            // get grid positions 
            Vector3Int drawPos = new Vector3Int(room.gridPos.x, room.gridPos.y, 0);

            // get world positions
            drawPos.x *= roomwidth + 1;
            drawPos.y *= roomheight + 1;

            // Spawn the room 
            DrawPrefab(drawPos, room.type.ToString());

        }
    }

    public void DrawPrefab(Vector3Int _gridPos, string _type)
    {
        string currentName = _type;

        // Iterate tilemaps to copy over roomprefab tiles
        foreach (TilemapClass prefab in tileMapPrefabs)
        {
            if (prefab.name != currentName)
            {
                //Debug.LogWarning(prefab.name + " Doesnt match anything.");
                continue;
            }

            //// Choose what roomtype to generate ( Unnecessary )
            //Room currentRoom = new Room(position, _type, prefab.width, prefab.height); // create a new room
            //rooms.Add(currentRoom); // add room to the list of rooms

            // get tiles to copy
            prefab.tilemap.CompressBounds();
            BoundsInt prefabBounds = prefab.tilemap.cellBounds;
            TileBase[] tileArray = prefab.tilemap.GetTilesBlock(prefabBounds);
            //Debug.Log(tileArray.Length);

            // place copied tiles into the world
            tilemap.CompressBounds();
            Vector3Int tilemapPos = new Vector3Int(_gridPos.x + -(prefab.width / 2), _gridPos.y + -(prefab.height / 2), 0);
            BoundsInt tilemapBounds = new BoundsInt(tilemapPos, prefab.tilemap.size);
            tilemap.SetTilesBlock(tilemapBounds, tileArray);

            // place gameobjects in world
        }
    }
    private void DrawCorridor(Room room, Vector3Int roomPos, Corridor.Direction direction)
    {
        Vector3Int offset = Vector3Int.zero;

        if (direction == Corridor.Direction.north)
        {
            //offset = Vector3Int.zero;
            //offset = new Vector3Int(room.gridPos.x * room.width, room.gridPos.y * room.height);
            offset = new Vector3Int((room.width / 2) + 1,(room.height / 2) + 1, 0);
        }
        if (direction == Corridor.Direction.east)
        {
            //offset = Vector3Int.zero;
            //offset = new Vector3Int(room.gridPos.x * room.width, room.gridPos.y * room.height);
            offset = new Vector3Int((room.width / 2) + 1, (room.height / 2) + 1, 0);
        }
        if (direction == Corridor.Direction.south)
        {
            //offset = Vector3Int.zero;
            //offset = new Vector3Int(room.gridPos.x * room.width, room.gridPos.y * room.height);
            offset = new Vector3Int((room.width / 2) + 1, (room.height / 2) + 1, 0);
        }
        if (direction == Corridor.Direction.west)
        {
            //offset = Vector3Int.zero;
            //offset = new Vector3Int(room.gridPos.x * room.width, room.gridPos.y * room.height);
            offset = new Vector3Int((room.width / 2) + 1, (room.height / 2) + 1, 0);
        }

        Vector3Int roomWorldPos = new Vector3Int(roomPos.x * room.width, roomPos.y * room.height, 0);

        Vector3Int drawPos = roomWorldPos + offset;

        //drawPos.x *= room.width;
        //drawPos.y *= room.height;

        // draw the right tilemap
        foreach(TilemapClass tilemap in tileMapPrefabs)
        {
            if (!tilemap.name.Contains("Corridor"))
            {
                continue;
            }

            // Spawn the corridor
            if(tilemap.name == "Corridor_Horizontal")
            {
                if (spawnDebugCorridors)
                {
                    //DrawPrefab(drawPos, "Corridor_Horizontal");
                    //Debug.Log("Corridors_Tried to draw horizontal corridor");
                    Instantiate(debugRoom, drawPos, Quaternion.identity);
                }
            }
            else if(tilemap.name == "Corridor_Vertical")
            {
                if (spawnDebugCorridors)
                {
                    //DrawPrefab(drawPos, "Corridor_Vertical");
                    //Debug.Log("Corridors_Tried to draw vertical corridor");
                    Instantiate(debugRoom, drawPos, Quaternion.identity);
                }
            }
        }

    }

    public void CreateCorridors()
    {
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                //Debug.Log("room is null");
                continue;
            }

            // get corridors
            List<Corridor> corridors = GetCorridors(room);

            foreach (Corridor corridor in corridors)
            {
                // get room grid positions 
                Vector3Int roomPos = new Vector3Int(room.gridPos.x, room.gridPos.y, 0);

                if (corridor.direction == Corridor.Direction.north)
                {
                    DrawCorridor(room, roomPos, corridor.direction);
                }
                if (corridor.direction == Corridor.Direction.east)
                {
                    DrawCorridor(room, roomPos, corridor.direction);
                }
                if (corridor.direction == Corridor.Direction.south)
                {
                    DrawCorridor(room, roomPos, corridor.direction);
                }
                if (corridor.direction == Corridor.Direction.west)
                {
                    DrawCorridor(room, roomPos, corridor.direction);
                }
            }
        }
    }

    private List<Corridor> GetCorridors(Room _room)
    {
        List<Corridor> allCorridors = new List<Corridor>();

        if (_room.north) // if the room has a corridor above
        { 
            allCorridors.Add(new Corridor(Corridor.Direction.north)); 
        }
        if (_room.east) // if the room has a corridor to the right
        {
            allCorridors.Add(new Corridor(Corridor.Direction.east));
        }
        if (_room.south) // if the room has a corridor below
        {
            allCorridors.Add(new Corridor(Corridor.Direction.south));
        }
        if (_room.west) // if the room has a corridor to the left
        {
            allCorridors.Add(new Corridor(Corridor.Direction.north));
        }

        return allCorridors;
    }

    private void SpawnFinalChamber()
    {
        // get random room
        Vector2Int finalChamberConnectionPos = takenPositions[Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1))];

        // get new room position
        Vector2Int finalChamberPos = Vector2Int.zero;

        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.left)) {}
        else if(finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.left;
        }

        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.right)) { }
        else if (finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.right;
        }

        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.up)) { }
        else if(finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.up;
        }
        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.down)) {}
        else if(finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.down;
        }

        Debug.Log("Final chamber pos: " + finalChamberPos);

        // add room to the room list
        if (finalChamberPos != Vector2Int.zero)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(MathF.Abs(finalChamberPos.x)), Mathf.RoundToInt(MathF.Abs(finalChamberPos.y)));

            rooms[pos.x, pos.y] = new Room(finalChamberPos, Room.RoomType.end, 20, 10);
            Debug.Log("final room position is: " + finalChamberPos.x + " , " + finalChamberPos.y);
        }
        else
        {
            Debug.LogWarning("No final chamber position found");
        }
    }

    private void SpawnShopChamber()
    {
        // get random room
        Vector2Int finalChamberConnectionPos = takenPositions[Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1))];

        // get new room position
        Vector2Int finalChamberPos = Vector2Int.zero;

        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.left)) { }
        else if (finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.left;
        }

        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.right)) { }
        else if (finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.right;
        }

        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.up)) { }
        else if (finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.up;
        }
        if (takenPositions.Contains(finalChamberConnectionPos + Vector2Int.down)) { }
        else if (finalChamberPos == Vector2Int.zero)
        {
            finalChamberPos = finalChamberConnectionPos + Vector2Int.down;
        }

        Debug.Log("Final chamber pos: " + finalChamberPos);

        // add room to the room list
        if (finalChamberPos != Vector2Int.zero)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(MathF.Abs(finalChamberPos.x)), Mathf.RoundToInt(MathF.Abs(finalChamberPos.y)));

            rooms[pos.x, pos.y] = new Room(finalChamberPos, Room.RoomType.shop, 20, 10);
            Debug.Log("final room position is: " + finalChamberPos.x + " , " + finalChamberPos.y);
        }
        else
        {
            Debug.LogWarning("No final chamber position found");
        }
    }
}
