using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Room Generation")]
    [SerializeField] GameObject roomPrefab;
    [SerializeField] int maxRooms = 15;
    [SerializeField] int minRooms = 7;

    [Header("Grid")]
    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;
    int roomWidth = 20;
    int roomHeight = 12;

    private int[,] roomGrid;
    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int roomCount;
    private bool generationComplete = false;

    void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            TryGenerateRoom(new Vector2Int(roomIndex.x + 1, roomIndex.y));
            TryGenerateRoom(new Vector2Int(roomIndex.x - 1, roomIndex.y));
            TryGenerateRoom(new Vector2Int(roomIndex.x, roomIndex.y + 1));
            TryGenerateRoom(new Vector2Int(roomIndex.x, roomIndex.y - 1));
        }
        else if (roomCount < minRooms)
        {
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            generationComplete = true;
            Debug.Log($"Room generation complete with {roomCount} rooms.");
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;
        GameObject initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        Room roomScript = initialRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        roomScript.IsStartingRoom = true;
        roomScript.RoomManager = this;
        initialRoom.name = $"Room_{roomCount}";
        roomObjects.Add(initialRoom);
        roomScript.EnableAllDoors(); // start doors enabled
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms) return false;
        if (Random.value < 0.5f && roomIndex != Vector2Int.zero) return false;
        if (CountAdjacentRooms(roomIndex) > 1) return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        GameObject newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        Room roomScript = newRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        roomScript.RoomManager = this;
        newRoom.name = $"Room_{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(roomScript, x, y);
        return true;
    }

    void OpenDoors(Room roomScript, int x, int y)
    {
        if (x > 0 && roomGrid[x - 1, y] != 0) { roomScript.OpenDoor(Vector2Int.left); GetRoomScriptAt(new Vector2Int(x - 1, y))?.OpenDoor(Vector2Int.right); }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) { roomScript.OpenDoor(Vector2Int.right); GetRoomScriptAt(new Vector2Int(x + 1, y))?.OpenDoor(Vector2Int.left); }
        if (y > 0 && roomGrid[x, y - 1] != 0) { roomScript.OpenDoor(Vector2Int.down); GetRoomScriptAt(new Vector2Int(x, y - 1))?.OpenDoor(Vector2Int.up); }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) { roomScript.OpenDoor(Vector2Int.up); GetRoomScriptAt(new Vector2Int(x, y + 1))?.OpenDoor(Vector2Int.down); }
    }

    public Room GetRoomScriptAt(Vector2Int index)
    {
        return roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index)?.GetComponent<Room>();
    }

    public void MovePlayerToRoom(Room fromRoom, Vector2Int direction)
    {
        Room targetRoom = GetRoomScriptAt(fromRoom.RoomIndex + direction);
        if (targetRoom == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = targetRoom.GetDoorPosition(-direction);
        targetRoom.DisableDoorsTemporarily(0.2f);
        targetRoom.OnPlayerEnter();
        Camera.main.transform.position = new Vector3(targetRoom.transform.position.x, targetRoom.transform.position.y, -10f); // snap camera
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int count = 0;
        int x = roomIndex.x;
        int y = roomIndex.y;
        if (x > 0 && roomGrid[x - 1, y] != 0) count++;
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++;
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++;
        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        return new Vector3(roomWidth * (gridIndex.x - gridSizeX / 2), roomHeight * (gridIndex.y - gridSizeY / 2), 0);
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
                Gizmos.DrawWireCube(GetPositionFromGridIndex(new Vector2Int(x, y)), new Vector3(roomWidth, roomHeight, 1));
    }
}
