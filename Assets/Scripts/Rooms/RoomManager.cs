using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;
    public GameObject bossPrefab;
    public GameObject bossHealthBarPrefab;


    int roomWidth = 30;
    int roomHeight = 12;

   [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;

    private int roomCount;
    private bool generationComplete = false;

    public GameObject trapdoorOpenPrefab;
    public GameObject trapdoorClosedPrefab;

    public MinimapManager minimap;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        minimap.Initialize(gridSizeX, gridSizeY); // <-- NEW

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
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

            // --- NEW: Spawn Boss ---
            GameObject boss = Instantiate(bossPrefab, lastRoom.transform.position, Quaternion.identity);
            boss.transform.SetParent(lastRoom.transform);

            // --- NEW: Spawn Boss Health Bar ---
            GameObject bossBar = Instantiate(bossHealthBarPrefab);
            EnemyHealth bossHealth = boss.GetComponent<EnemyHealth>();
            BossHealthBar barScript = bossBar.GetComponent<BossHealthBar>();
            barScript.bossHealth = bossHealth;
        }

    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;

        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        initialRoom.GetComponent<Room>().isStartingRoom = true;

        roomObjects.Add(initialRoom);

        minimap.RegisterRoom(roomIndex);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms) return false;
        if (Random.value > 0.5f && roomIndex != Vector2Int.zero) return false;
        if (CountAdjacentRooms(roomIndex) > 1) return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        minimap.RegisterRoom(roomIndex);

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
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            newRoomScript.hasLeftDoor = true;
            leftRoomScript.hasRightDoor = true;

            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }

        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            newRoomScript.hasRightDoor = true;
            rightRoomScript.hasLeftDoor = true;

            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }

        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            newRoomScript.hasBottomDoor = true;
            bottomRoomScript.hasTopDoor = true;

            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }

        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            newRoomScript.hasTopDoor = true;
            topRoomScript.hasBottomDoor = true;

            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
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

        if (x > 0 && roomGrid[x - 1, y] != 0) count++;
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++;
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Draw wireframes for all generated rooms (only works in play mode)
        if (roomObjects != null)
        {
            foreach (var room in roomObjects)
            {
                if (room != null)
                {
                    Gizmos.DrawWireCube(
                        room.transform.position,
                        new Vector3(roomWidth, roomHeight, 0.1f)
                    );
                }
            }
        }

        // Draw the grid center (useful for minimap alignment)
        Gizmos.color = Color.cyan;
        Vector3 centerPos = new Vector3(
            roomWidth * (gridSizeX / 2 - gridSizeX / 2),
            roomHeight * (gridSizeY / 2 - gridSizeY / 2),
            0
        );
        Gizmos.DrawSphere(centerPos, 0.5f);
    }

}
