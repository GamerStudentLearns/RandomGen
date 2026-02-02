using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

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

    [Header("Boss Prefab")]
    public GameObject bossPrefab;

    [Header("Special Room Prefabs")]
    [SerializeField] private GameObject[] specialRoomPrefabs;

    private static HashSet<GameObject> spawnedItemsThisRun = new HashSet<GameObject>();

    public MinimapManager minimap;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            spawnedItemsThisRun.Clear();
        }
    }

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        minimap.Initialize(gridSizeX, gridSizeY);

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

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
            StartCoroutine(FinalizeGeneration());
        }
    }

    private System.Collections.IEnumerator FinalizeGeneration()
    {
        yield return null;

        // ---------------------------------------------------------
        // 1. Assign trapdoor + boss to the last generated room
        // ---------------------------------------------------------
        GameObject lastRoom = roomObjects.Last();
        Room lastRoomScript = lastRoom.GetComponent<Room>();

        // Mark room as having a trapdoor
        lastRoomScript.hasTrapdoor = true;

        // -------------------------
        // Spawn trapdoor (open + closed)
        // -------------------------
        GameObject trapOpen = Instantiate(trapdoorOpenPrefab, lastRoom.transform.position, Quaternion.identity, lastRoom.transform);
        GameObject trapClosed = Instantiate(trapdoorClosedPrefab, lastRoom.transform.position, Quaternion.identity, lastRoom.transform);

        lastRoomScript.trapdoorOpen = trapOpen;
        lastRoomScript.trapdoorClosed = trapClosed;

        trapOpen.SetActive(false);
        trapClosed.SetActive(true);

        // -------------------------
        // Spawn the boss
        // -------------------------
        if (bossPrefab != null)
        {
            // Offset so boss doesn't overlap trapdoor
            Vector3 bossPos = lastRoom.transform.position + new Vector3(0, 2f, 0);

            GameObject boss = Instantiate(bossPrefab, bossPos, Quaternion.identity, lastRoom.transform);

            // Assign to room
            lastRoomScript.bossObject = boss;
            lastRoomScript.hasBoss = true;

            // Optional: allow boss to notify room when it dies
            EnemyHealth health = boss.GetComponent<EnemyHealth>();
            if (health != null)
                health.parentRoom = lastRoomScript;
        }

        // ---------------------------------------------------------
        // 2. Collect all one-door rooms (excluding start + last)
        // ---------------------------------------------------------
        List<GameObject> oneDoorRooms = new List<GameObject>();

        foreach (var roomObj in roomObjects)
        {
            Room room = roomObj.GetComponent<Room>();

            if (room.isStartingRoom) continue;
            if (roomObj == lastRoom) continue;

            if (CountDoors(room) == 1)
                oneDoorRooms.Add(roomObj);
        }

        // ---------------------------------------------------------
        // 3. Pick a special room prefab that hasn't been used
        // ---------------------------------------------------------
        var availablePrefabs = specialRoomPrefabs
            .Where(p => !spawnedItemsThisRun.Contains(p))
            .ToList();

        if (oneDoorRooms.Count > 0 && availablePrefabs.Count > 0)
        {
            // Choose the one-door room
            GameObject chosenRoom = oneDoorRooms[Random.Range(0, oneDoorRooms.Count)];
            Room chosenRoomScript = chosenRoom.GetComponent<Room>();

            // Choose the special prefab
            GameObject chosenPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];

            // Spawn the special room content inside it
            Instantiate(chosenPrefab, chosenRoom.transform.position, Quaternion.identity, chosenRoom.transform);

            spawnedItemsThisRun.Add(chosenPrefab);

            // ---------------------------------------------------------
            // 4. Apply special door sprites to this room
            // ---------------------------------------------------------
            chosenRoomScript.ApplySpecialDoorSprites();

            // ---------------------------------------------------------
            // 5. Apply opposite-facing special sprites to the connecting room
            // ---------------------------------------------------------
            ApplySpecialSpritesToConnectingRoom(chosenRoomScript);
        }
    }


    private void ApplySpecialSpritesToConnectingRoom(Room specialRoom)
    {
        Vector2Int index = specialRoom.RoomIndex;

        // Special room has a TOP door → neighbor is above → neighbor gets BOTTOM special sprite
        if (specialRoom.hasTopDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.up, "Bottom");

        // Special room has a BOTTOM door → neighbor is below → neighbor gets TOP special sprite
        if (specialRoom.hasBottomDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.down, "Top");

        // Special room has a LEFT door → neighbor is left → neighbor gets RIGHT special sprite
        if (specialRoom.hasLeftDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.left, "Right");

        // Special room has a RIGHT door → neighbor is right → neighbor gets LEFT special sprite
        if (specialRoom.hasRightDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.right, "Left");
    }
    private void ApplyOppositeSpecialSprite(Vector2Int neighborIndex, string oppositeDirection)
    {
        Room neighbor = GetRoomScriptAt(neighborIndex);
        if (neighbor == null) return;

        neighbor.useSpecialDoorSprites = true;

        SpriteRenderer openRenderer = null;
        SpriteRenderer closedRenderer = null;

        switch (oppositeDirection)
        {
            case "Top":
                openRenderer = neighbor.topDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.topClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.specialTopDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.specialTopClosedSprite;
                break;

            case "Bottom":
                openRenderer = neighbor.bottomDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.bottomClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.specialBottomDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.specialBottomClosedSprite;
                break;

            case "Left":
                openRenderer = neighbor.leftDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.leftClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.specialLeftDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.specialLeftClosedSprite;
                break;

            case "Right":
                openRenderer = neighbor.rightDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.rightClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.specialRightDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.specialRightClosedSprite;
                break;
        }
    }




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

    private int CountDoors(Room room)
    {
        int doors = 0;
        if (room.hasLeftDoor) doors++;
        if (room.hasRightDoor) doors++;
        if (room.hasTopDoor) doors++;
        if (room.hasBottomDoor) doors++;
        return doors;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }
}
