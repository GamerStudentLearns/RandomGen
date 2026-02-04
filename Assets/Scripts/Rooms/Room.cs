using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Trigger (assign manually)")]
    public Collider2D roomTrigger;

    [Header("Doors (assigned in prefab)")]
    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;

    [Header("Closed Door Sprites")]
    public GameObject topClosedDoor;
    public GameObject bottomClosedDoor;
    public GameObject leftClosedDoor;
    public GameObject rightClosedDoor;

    [Header("Special Door Sprites (Optional)")]
    public Sprite specialTopDoorSprite;
    public Sprite specialBottomDoorSprite;
    public Sprite specialLeftDoorSprite;
    public Sprite specialRightDoorSprite;

    public Sprite specialTopClosedSprite;
    public Sprite specialBottomClosedSprite;
    public Sprite specialLeftClosedSprite;
    public Sprite specialRightClosedSprite;

    public bool useSpecialDoorSprites = false;

    [Header("Trapdoor Objects")]
    public GameObject trapdoorOpen;
    public GameObject trapdoorClosed;
    public bool hasTrapdoor;

    [Header("Boss Room")]
    public bool isBossRoom = false;
    public GameObject bossObject;   // Assigned by RoomManager
    public bool hasBoss = false;

    [Header("Door Existence Flags (set by RoomManager)")]
    public bool hasTopDoor;
    public bool hasBottomDoor;
    public bool hasLeftDoor;
    public bool hasRightDoor;

    [Header("Room Settings")]
    public bool isStartingRoom = false;

    [Header("Lock Behaviour")]
    public GameObject[] objectsToDisableOnLock;
    public GameObject[] objectsToEnableOnLock;

    [Header("Clear Behaviour")]
    public GameObject[] objectsToDisableOnClear;
    public GameObject[] objectsToEnableOnClear;

    [Header("Enemy Spawning")]
    public List<Transform> enemySpawnPoints;
    public List<GameObject> enemyPrefabs;

    public Vector2Int RoomIndex { get; set; }

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool roomActivated = false;

    private MinimapManager minimap;
    private MinimapIcon minimapIcon;

    private void Awake()
    {
        if (roomTrigger != null)
        {
            roomTrigger.isTrigger = true;

            RoomTriggerForwarder forwarder = roomTrigger.gameObject.AddComponent<RoomTriggerForwarder>();
            forwarder.parentRoom = this;
        }

        minimap = FindFirstObjectByType<MinimapManager>();
    }

    private void Start()
    {
        minimapIcon = minimap.GetIcon(RoomIndex);

        RockSpawner rocks = GetComponent<RockSpawner>();
        if (rocks != null)
            rocks.TrySpawnRocks();
    }

    public void PlayerEnteredRoom()
    {
        // Reveal this room on the minimap
        if (minimapIcon != null)
            minimapIcon.Reveal();

        minimap.MarkVisited(RoomIndex);
        minimap.SetCurrentRoom(RoomIndex);

        // Reveal adjacent rooms
        RevealAdjacent(RoomIndex + Vector2Int.up);
        RevealAdjacent(RoomIndex + Vector2Int.down);
        RevealAdjacent(RoomIndex + Vector2Int.left);
        RevealAdjacent(RoomIndex + Vector2Int.right);

        // Prevent double activation
        if (roomActivated)
            return;

        roomActivated = true;

        // Starting room never locks or spawns enemies
        if (isStartingRoom)
            return;

        // Lock the room (doors close)
        LockRoom();

        // Spawn normal enemies only in non-boss rooms
        if (!isBossRoom)
            SpawnEnemies();

        // Boss activation (RoomManager already spawned the boss)
        if (isBossRoom && bossObject != null)
        {
            // Hook boss health to room + UI
            EnemyHealth bossHealth = bossObject.GetComponent<EnemyHealth>();
            if (bossHealth != null)
            {
                bossHealth.parentRoom = this;
                BossHealthUI.instance.Show(bossHealth.maxHealth);
            }

            // Wake the boss AI
            IBoss[] bosses = bossObject.GetComponents<IBoss>();
            foreach (IBoss boss in bosses)
                boss.WakeUp();

        }

        // Begin monitoring for room clear
        StartCoroutine(CheckRoomClear());
    }


    private void RevealAdjacent(Vector2Int index)
    {
        MinimapIcon icon = minimap.GetIcon(index);
        if (icon == null)
            return;

        icon.Reveal();

        if (minimap.IsVisited(index))
            icon.SetVisited();
        else
            icon.SetUnvisited();
    }

    private void LockRoom()
    {
        foreach (var obj in objectsToDisableOnLock)
            if (obj != null && ShouldAffectDoor(obj))
                obj.SetActive(false);

        foreach (var obj in objectsToEnableOnLock)
            if (obj != null && ShouldAffectDoor(obj))
                obj.SetActive(true);

        if (hasTrapdoor)
        {
            if (trapdoorOpen != null) trapdoorOpen.SetActive(false);
            if (trapdoorClosed != null) trapdoorClosed.SetActive(true);
        }
    }

    private void ClearRoom()
    {
        foreach (var obj in objectsToDisableOnClear)
            if (obj != null && ShouldAffectDoor(obj))
                obj.SetActive(false);

        foreach (var obj in objectsToEnableOnClear)
            if (obj != null && ShouldAffectDoor(obj))
                obj.SetActive(true);

        if (hasTrapdoor)
        {
            if (trapdoorClosed != null) trapdoorClosed.SetActive(false);
            if (trapdoorOpen != null) trapdoorOpen.SetActive(true);
        }

        PickupSpawner spawner = GetComponent<PickupSpawner>();
        if (spawner != null)
            spawner.TrySpawnPickup();
    }

    private bool ShouldAffectDoor(GameObject obj)
    {
        if ((obj == topDoor || obj == topClosedDoor) && !hasTopDoor) return false;
        if ((obj == bottomDoor || obj == bottomClosedDoor) && !hasBottomDoor) return false;
        if ((obj == leftDoor || obj == leftClosedDoor) && !hasLeftDoor) return false;
        if ((obj == rightDoor || obj == rightClosedDoor) && !hasRightDoor) return false;

        if ((obj == trapdoorOpen || obj == trapdoorClosed) && !hasTrapdoor) return false;

        return true;
    }

    private void SpawnEnemies()
    {
        if (isStartingRoom)
            return;

        if (isBossRoom)
            return;

        if (enemyPrefabs.Count == 0 || enemySpawnPoints.Count == 0)
            return;

        int maxPossible = enemySpawnPoints.Count;
        int enemyCount = Random.Range(0, maxPossible + 1);

        List<Transform> availableSpawns = new List<Transform>(enemySpawnPoints);

        for (int i = 0; i < enemyCount; i++)
        {
            int spawnIndex = Random.Range(0, availableSpawns.Count);
            Transform spawn = availableSpawns[spawnIndex];
            availableSpawns.RemoveAt(spawnIndex);

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.parentRoom = this;

            spawnedEnemies.Add(enemy);
        }
    }

    private IEnumerator CheckRoomClear()
    {
        while (true)
        {
            spawnedEnemies.RemoveAll(e => e == null);

            bool bossDead = true;

            if (hasBoss)
                bossDead = (bossObject == null);

            if (spawnedEnemies.Count == 0 && bossDead)
            {
                ClearRoom();
                yield break;
            }

            yield return null;
        }
    }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up && topDoor != null)
            topDoor.SetActive(true);

        if (direction == Vector2Int.down && bottomDoor != null)
            bottomDoor.SetActive(true);

        if (direction == Vector2Int.left && leftDoor != null)
            leftDoor.SetActive(true);

        if (direction == Vector2Int.right && rightDoor != null)
            rightDoor.SetActive(true);
    }

    public void ApplySpecialDoorSprites()
    {
        useSpecialDoorSprites = true;

        if (hasTopDoor && topDoor != null)
            topDoor.GetComponent<SpriteRenderer>().sprite = specialTopDoorSprite;

        if (hasBottomDoor && bottomDoor != null)
            bottomDoor.GetComponent<SpriteRenderer>().sprite = specialBottomDoorSprite;

        if (hasLeftDoor && leftDoor != null)
            leftDoor.GetComponent<SpriteRenderer>().sprite = specialLeftDoorSprite;

        if (hasRightDoor && rightDoor != null)
            rightDoor.GetComponent<SpriteRenderer>().sprite = specialRightDoorSprite;

        if (hasTopDoor && topClosedDoor != null)
            topClosedDoor.GetComponent<SpriteRenderer>().sprite = specialTopClosedSprite;

        if (hasBottomDoor && bottomClosedDoor != null)
            bottomClosedDoor.GetComponent<SpriteRenderer>().sprite = specialBottomClosedSprite;

        if (hasLeftDoor && leftClosedDoor != null)
            leftClosedDoor.GetComponent<SpriteRenderer>().sprite = specialLeftClosedSprite;

        if (hasRightDoor && rightClosedDoor != null)
            rightClosedDoor.GetComponent<SpriteRenderer>().sprite = specialRightClosedSprite;
    }
}
