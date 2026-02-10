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
    public static RoomManager Instance;

    [Header("Optional Exit Prefab")]
    public GameObject optionalExitPrefab;
    public bool spawnOptionalExit = true;

    [Header("Boss Prefabs")]
    public GameObject[] bossPrefab;

    [Header("Special Floor Settings")]
    public bool replaceEnemiesWithBosses = false;
    public GameObject[] miniBossPrefabs;

    [Header("Special Room Prefabs")]
    [SerializeField] private GameObject[] specialRoomPrefabs;

    [Header("Boss Reward Items")]
    public GameObject[] bossRewardItemPrefabs;




    private static HashSet<GameObject> spawnedItemsThisRun = new HashSet<GameObject>();
    private static HashSet<GameObject> usedBossesThisRun = new HashSet<GameObject>();
    public static HashSet<GameObject> bossRewardsUsedThisRun = new HashSet<GameObject>();


    public MinimapManager minimap;

    public void Awake()
    {
        Instance = this;
    }

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
            usedBossesThisRun.Clear();
            bossRewardsUsedThisRun.Clear();   // NEW
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

        // 1. Pick the REAL boss room (furthest *1-door* room)
        GameObject lastRoom = GetFurthestDeadEndRoom();
        Room lastRoomScript = lastRoom.GetComponent<Room>();

        lastRoomScript.isBossRoom = true;
        lastRoomScript.hasBoss = true;
        lastRoomScript.roomType = Room.RoomType.Boss;

        // *** UPDATE MINIMAP ICON ***
        MinimapIcon bossIcon = minimap.GetIcon(lastRoomScript.RoomIndex);
        if (bossIcon != null)
        {
            bossIcon.iconType = MinimapIcon.IconType.Boss;
            bossIcon.SetUnvisited();
        }

        // 2. Choose a boss prefab that hasn't been used this run
        var availableBosses = bossPrefab
            .Where(b => !usedBossesThisRun.Contains(b))
            .ToList();

        if (availableBosses.Count == 0)
            availableBosses = bossPrefab.ToList();

        GameObject chosenBoss = availableBosses[Random.Range(0, availableBosses.Count)];
        usedBossesThisRun.Add(chosenBoss);

        // 3. Spawn boss
        GameObject bossInstance = Instantiate(
            chosenBoss,
            lastRoom.transform.position + new Vector3(0, 2f, 0),
            Quaternion.identity,
            lastRoom.transform
        );

        lastRoomScript.bossObject = bossInstance;

        // 4. Spawn trapdoor
        GameObject trapOpen = Instantiate(
            trapdoorOpenPrefab,
            lastRoom.transform.position,
            Quaternion.identity,
            lastRoom.transform
        );

        GameObject trapClosed = Instantiate(
            trapdoorClosedPrefab,
            lastRoom.transform.position,
            Quaternion.identity,
            lastRoom.transform
        );

        lastRoomScript.trapdoorOpen = trapOpen;
        lastRoomScript.trapdoorClosed = trapClosed;

        trapOpen.SetActive(false);
        trapClosed.SetActive(true);

        lastRoomScript.hasTrapdoor = true;

        // 5. Optional exit
        if (spawnOptionalExit && optionalExitPrefab != null)
        {
            GameObject optionalExit = Instantiate(
                optionalExitPrefab,
                lastRoom.transform.position + new Vector3(3f, 0, 0),
                Quaternion.identity,
                lastRoom.transform
            );

            optionalExit.SetActive(false);

            lastRoomScript.optionalExitObject = optionalExit;
            lastRoomScript.hasOptionalExit = true;
        }

        // 6. Apply boss door sprites
        lastRoomScript.ApplyBossDoorSprites();
        ApplyBossSpritesToConnectingRooms(lastRoomScript);

        // 7. Collect all one-door rooms (excluding start + boss)
        List<GameObject> oneDoorRooms = new List<GameObject>();

        foreach (var roomObj in roomObjects)
        {
            Room room = roomObj.GetComponent<Room>();

            if (room.isStartingRoom) continue;
            if (roomObj == lastRoom) continue;

            if (CountDoors(room) == 1)
                oneDoorRooms.Add(roomObj);
        }

        // 8. Spawn special room
        var availablePrefabs = specialRoomPrefabs
            .Where(p => !spawnedItemsThisRun.Contains(p))
            .ToList();

        if (oneDoorRooms.Count > 0 && availablePrefabs.Count > 0)
        {
            GameObject chosenRoom = oneDoorRooms[Random.Range(0, oneDoorRooms.Count)];
            Room chosenRoomScript = chosenRoom.GetComponent<Room>();

            GameObject chosenPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];

            Instantiate(chosenPrefab, chosenRoom.transform.position, Quaternion.identity, chosenRoom.transform);

            spawnedItemsThisRun.Add(chosenPrefab);

            chosenRoomScript.ApplySpecialDoorSprites();
            ApplySpecialSpritesToConnectingRoom(chosenRoomScript);

            chosenRoomScript.roomType = Room.RoomType.Item;

            // *** UPDATE MINIMAP ICON ***
            MinimapIcon itemIcon = minimap.GetIcon(chosenRoomScript.RoomIndex);
            if (itemIcon != null)
            {
                itemIcon.iconType = MinimapIcon.IconType.Item;
                itemIcon.SetUnvisited();
            }
        }
    }



    // ------------------------------
    // NEW: Furthest 1-door room logic
    // ------------------------------
    private GameObject GetFurthestDeadEndRoom()
    {
        Vector2Int start = new Vector2Int(gridSizeX / 2, gridSizeY / 2);

        GameObject furthest = null;
        float maxDist = -1f;

        foreach (var roomObj in roomObjects)
        {
            Room r = roomObj.GetComponent<Room>();
            if (r.isStartingRoom) continue;

            if (CountDoors(r) != 1) continue;

            float dist = Vector2Int.Distance(r.RoomIndex, start);

            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = roomObj;
            }
        }

        if (furthest != null)
            return furthest;

        return GetFurthestRoom();
    }

    private GameObject GetFurthestRoom()
    {
        Vector2Int start = new Vector2Int(gridSizeX / 2, gridSizeY / 2);

        GameObject furthest = null;
        float maxDist = -1f;

        foreach (var roomObj in roomObjects)
        {
            Room r = roomObj.GetComponent<Room>();
            if (r.isStartingRoom) continue;

            float dist = Vector2Int.Distance(r.RoomIndex, start);

            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = roomObj;
            }
        }

        return furthest;
    }

    // ------------------------------
    // BOSS DOOR NEIGHBOR PROPAGATION
    // ------------------------------
    private void ApplyBossSpritesToConnectingRooms(Room bossRoom)
    {
        Vector2Int index = bossRoom.RoomIndex;

        if (bossRoom.hasTopDoor)
            ApplyOppositeBossSprite(index + Vector2Int.up, "Bottom");

        if (bossRoom.hasBottomDoor)
            ApplyOppositeBossSprite(index + Vector2Int.down, "Top");

        if (bossRoom.hasLeftDoor)
            ApplyOppositeBossSprite(index + Vector2Int.left, "Right");

        if (bossRoom.hasRightDoor)
            ApplyOppositeBossSprite(index + Vector2Int.right, "Left");
    }

    private void ApplyOppositeBossSprite(Vector2Int neighborIndex, string oppositeDirection)
    {
        Room neighbor = GetRoomScriptAt(neighborIndex);
        if (neighbor == null) return;

        neighbor.useBossDoorSprites = true;

        SpriteRenderer openRenderer = null;
        SpriteRenderer closedRenderer = null;

        switch (oppositeDirection)
        {
            case "Top":
                openRenderer = neighbor.topDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.topClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.bossTopDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.bossTopClosedSprite;
                break;

            case "Bottom":
                openRenderer = neighbor.bottomDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.bottomClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.bossBottomDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.bossBottomClosedSprite;
                break;

            case "Left":
                openRenderer = neighbor.leftDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.leftClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.bossLeftDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.bossLeftClosedSprite;
                break;

            case "Right":
                openRenderer = neighbor.rightDoor?.GetComponent<SpriteRenderer>();
                closedRenderer = neighbor.rightClosedDoor?.GetComponent<SpriteRenderer>();
                if (openRenderer != null) openRenderer.sprite = neighbor.bossRightDoorSprite;
                if (closedRenderer != null) closedRenderer.sprite = neighbor.bossRightClosedSprite;
                break;
        }
    }

    // ------------------------------
    // SPECIAL ROOM NEIGHBOR PROPAGATION
    // ------------------------------
    private void ApplySpecialSpritesToConnectingRoom(Room specialRoom)
    {
        Vector2Int index = specialRoom.RoomIndex;

        if (specialRoom.hasTopDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.up, "Bottom");

        if (specialRoom.hasBottomDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.down, "Top");

        if (specialRoom.hasLeftDoor)
            ApplyOppositeSpecialSprite(index + Vector2Int.left, "Right");

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

    // ------------------------------
    // ROOM GENERATION LOGIC
    // ------------------------------
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

    public Room GetRoomScriptAt(Vector2Int index)
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
