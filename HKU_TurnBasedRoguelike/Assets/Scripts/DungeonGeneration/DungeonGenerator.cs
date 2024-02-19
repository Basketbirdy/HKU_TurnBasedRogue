using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Variables")]
    [Header("Rooms")]
    [SerializeField] List<Room> rooms = new List<Room>();
    [SerializeField] int count;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap tilemap;
    [Header("Prefabs")]
    [SerializeField] private TilemapClass[] tileMapPrefabs;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            Debug.Log("Room_Input received");
            CreateRoom(Vector2Int.zero, Room.RoomType.empty);
            count++;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Room_Input received");
            CreateRoom(new Vector2Int(0, count * 11), Room.RoomType.empty);
            count++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var room in rooms) {
            Vector3Int gridPos = new Vector3Int(room.gridPos.x, room.gridPos.y, 0);
            Gizmos.DrawWireCube(gridPos, new Vector3(room.width, room.height, 0)); 
        }
    }

    public void CreateRoom(Vector2Int position, Room.RoomType _type)
    {
        string currentName = "";

        switch (_type)
        {
            case Room.RoomType.start:
                currentName = "Room_Start";
                break;
            case Room.RoomType.end:
                currentName = "Room_End";
                break;
            case Room.RoomType.boss:
                currentName = "Room_Boss";
                break;
            case Room.RoomType.item:
                currentName = "Room_Item";
                break;
            case Room.RoomType.empty:
                currentName = "Room_Default";
                break;
        }

        // Iterate tilemaps to copy over roomprefab tiles
        foreach (TilemapClass prefab in tileMapPrefabs) 
        {
            if(prefab.name != currentName) 
            { 
                Debug.LogWarning(prefab.name + "Doesnt match anything.");
                break; 
            }

            // Choose what roomtype to generate
            Room currentRoom = new Room(position, _type, prefab.width, prefab.height); // create a new room
            rooms.Add(currentRoom); // add room to the list of rooms

            prefab.tilemap.CompressBounds();
            BoundsInt prefabBounds = prefab.tilemap.cellBounds;
            TileBase[] tileArray = prefab.tilemap.GetTilesBlock(prefabBounds);
            Debug.Log(tileArray.Length);

            tilemap.CompressBounds();
            Vector3Int tilemapPos = new Vector3Int(position.x + -(prefab.width/2), position.y + -(prefab.height / 2), 0);
            BoundsInt tilemapBounds = new BoundsInt(tilemapPos, prefab.tilemap.size);
            tilemap.SetTilesBlock(tilemapBounds, tileArray);
        }
    }
}
