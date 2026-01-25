using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    [Header("Room Prefabs")]
    [SerializeField] GameObject roomPrefab;

    [Header("Generation Settings")]
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;

    int roomWidth = 30;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;

    private int roomCount;
    private bool generationComplete = false;

    [Header("Trapdoor Prefabs")]
    public GameObject trapdoorOpenPrefab;
    public GameObject trapdoorClosedPrefab;

    public MinimapManager minimap;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        minimap.Initialize(gridSizeX, gridSizeY);

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        // ⭐ Generate the first wave of rooms immediately (still random)
        GenerateInitialNeighbors();
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (roomCount < minRooms && !generationComplete)
        {
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            generationComplete = true;

            GameObject lastRoom = roomObjects.Last();
            Room lastRoomScript = lastRoom.GetComponent<Room>();

            lastRoomScript.hasTrapdoor = true;

            GameObject trapOpen = Instantiate(trapdoorOpenPrefab, lastRoom.transform.position, Quaternion.identity);
            GameObject trapClosed = Instantiate(trapdoorClosedPrefab, lastRoom.transform.position, Quaternion.identity);

            trapOpen.transform.SetParent(lastRoom.transform);
            trapClosed.transform.SetParent(lastRoom.transform);

            lastRoomScript.trapdoorOpen = trapOpen;
            lastRoomScript.trapdoorClosed = trapClosed;

            trapOpen.SetActive(false);
            trapClosed.SetActive(true);
        }
    }

    // ⭐ NEW: Generate the first wave of neighbors but DO NOT reveal them
    private void GenerateInitialNeighbors()
    {
        if (roomQueue.Count == 0) return;

        Vector2Int start = roomQueue.Peek();
        int x = start.x;
        int y = start.y;

        TryGenerateRoom(new Vector2Int(x + 1, y));
        TryGenerateRoom(new Vector2Int(x - 1, y));
        TryGenerateRoom(new Vector2Int(x, y + 1));
        TryGenerateRoom(new Vector2Int(x, y - 1));
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;

        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        Room roomScript = initialRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        roomScript.isStartingRoom = true;

        roomObjects.Add(initialRoom);
        minimap.RegisterRoom(roomIndex);

        // ⭐ Starting room is revealed
        MinimapIcon startIcon = minimap.GetIcon(roomIndex);
        if (startIcon != null)
        {
            startIcon.Reveal();
            minimap.SetCurrentRoom(roomIndex);
        }
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return false;

        if (roomGrid[x, y] != 0)
            return false;

        if (roomCount >= maxRooms)
            return false;

        // ⭐ Randomness preserved
        if (Random.value > 0.5f)
            return false;

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        Room roomScript = newRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        minimap.RegisterRoom(roomIndex);

        // ⭐ Keep fog-of-war: DO NOT reveal
        MinimapIcon icon = minimap.GetIcon(roomIndex);
        if (icon != null)
            icon.Hide();

        OpenDoors(newRoom, x, y);

        return true;
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();

        minimap.ClearIcons();

        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        // ⭐ First wave again
        GenerateInitialNeighbors();
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
            if (leftRoomScript != null)
            {
                newRoomScript.hasLeftDoor = true;
                leftRoomScript.hasRightDoor = true;

                newRoomScript.OpenDoor(Vector2Int.left);
                leftRoomScript.OpenDoor(Vector2Int.right);
            }
        }

        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
            if (rightRoomScript != null)
            {
                newRoomScript.hasRightDoor = true;
                rightRoomScript.hasLeftDoor = true;

                newRoomScript.OpenDoor(Vector2Int.right);
                rightRoomScript.OpenDoor(Vector2Int.left);
            }
        }

        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));
            if (bottomRoomScript != null)
            {
                newRoomScript.hasBottomDoor = true;
                bottomRoomScript.hasTopDoor = true;

                newRoomScript.OpenDoor(Vector2Int.down);
                bottomRoomScript.OpenDoor(Vector2Int.up);
            }
        }

        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
            if (topRoomScript != null)
            {
                newRoomScript.hasTopDoor = true;
                topRoomScript.hasBottomDoor = true;

                newRoomScript.OpenDoor(Vector2Int.up);
                topRoomScript.OpenDoor(Vector2Int.down);
            }
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x - 1 >= 0 && roomGrid[x - 1, y] != 0) count++;
        if (x + 1 < gridSizeX && roomGrid[x + 1, y] != 0) count++;
        if (y - 1 >= 0 && roomGrid[x, y - 1] != 0) count++;
        if (y + 1 < gridSizeY && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }
}
