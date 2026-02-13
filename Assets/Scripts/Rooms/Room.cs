using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Room : MonoBehaviour
{
    public enum RoomType { Normal, Boss, Item }

    [Header("Room Type")]
    public RoomType roomType = RoomType.Normal;

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

    [Header("Boss Door Sprites (Optional)")]
    public Sprite bossTopDoorSprite;
    public Sprite bossBottomDoorSprite;
    public Sprite bossLeftDoorSprite;
    public Sprite bossRightDoorSprite;

    public Sprite bossTopClosedSprite;
    public Sprite bossBottomClosedSprite;
    public Sprite bossLeftClosedSprite;
    public Sprite bossRightClosedSprite;

    public bool useBossDoorSprites = false;

    [Header("Trapdoor Objects")]
    public GameObject trapdoorOpen;
    public GameObject trapdoorClosed;
    public bool hasTrapdoor;

    [Header("Optional Exit")]
    public GameObject optionalExitObject;
    public bool hasOptionalExit = false;

    [Header("Boss Room")]
    public bool isBossRoom = false;
    public GameObject bossObject;
    public bool hasBoss = false;

    [Header("Door Existence Flags (set by RoomManager)")]
    public bool hasTopDoor;
    public bool hasBottomDoor;
    public bool hasLeftDoor;
    public bool hasRightDoor;

    [Header("Room Settings")]
    public bool isStartingRoom = false;

    [Header("Boss Reward Control")]
    public bool bossRewardSpawned = false;


    [Header("Lock Behaviour")]
    public GameObject[] objectsToDisableOnLock;
    public GameObject[] objectsToEnableOnLock;

    [Header("Clear Behaviour")]
    public GameObject[] objectsToDisableOnClear;
    public GameObject[] objectsToEnableOnClear;

    [Header("Room Decor Points")]
    public List<Transform> floorDecorPoints;
    public List<Transform> topWallDecorPoints;
    public List<Transform> bottomWallDecorPoints;
    public List<Transform> leftWallDecorPoints;
    public List<Transform> rightWallDecorPoints;

    [Header("Room Decor Prefabs")]
    public List<GameObject> floorDecorPrefabs;
    public List<GameObject> topWallDecorPrefabs;
    public List<GameObject> bottomWallDecorPrefabs;
    public List<GameObject> leftWallDecorPrefabs;
    public List<GameObject> rightWallDecorPrefabs;
    // paintings, torches, banners


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

        SpawnDecor(); // ← correct place
    }


    public void PlayerEnteredRoom()
    {
        if (minimapIcon != null)
            minimapIcon.Reveal();

        minimap.MarkVisited(RoomIndex);
        minimap.SetCurrentRoom(RoomIndex);

        RevealAdjacent(RoomIndex + Vector2Int.up);
        RevealAdjacent(RoomIndex + Vector2Int.down);
        RevealAdjacent(RoomIndex + Vector2Int.left);
        RevealAdjacent(RoomIndex + Vector2Int.right);

        if (roomActivated)
            return;

        roomActivated = true;

        RoomEvents.RoomEntered(this);   // EVENT HOOK

        if (isStartingRoom)
            return;

        LockRoom();

        if (!isBossRoom)
            SpawnEnemies();

        if (isBossRoom && bossObject != null)
        {
            EnemyHealth bossHealth = bossObject.GetComponent<EnemyHealth>();
            if (bossHealth != null)
            {
                bossHealth.parentRoom = this;
                BossHealthUI.instance.Show(bossHealth.maxHealth);
            }

            IBoss[] bosses = bossObject.GetComponents<IBoss>();
            foreach (IBoss boss in bosses)
                boss.WakeUp();
        }

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

        if (hasOptionalExit && optionalExitObject != null)
            optionalExitObject.SetActive(false);
    }

    private void ClearRoom()
    {
        RoomEvents.RoomCleared(this);

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
            spawner.TrySpawnPickup(isBossRoom);

        if (hasOptionalExit && optionalExitObject != null)
            optionalExitObject.SetActive(true);

        if (isBossRoom && !bossRewardSpawned)
        {
            SpawnBossReward();
            bossRewardSpawned = true;
        }
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

        if (enemySpawnPoints.Count == 0)
            return;

        bool spawnBosses = RoomManager.Instance.replaceEnemiesWithBosses;

        int maxPossible = enemySpawnPoints.Count;
        int enemyCount = Random.Range(0, maxPossible + 1);

        List<Transform> availableSpawns = new List<Transform>(enemySpawnPoints);

        for (int i = 0; i < enemyCount; i++)
        {
            int spawnIndex = Random.Range(0, availableSpawns.Count);
            Transform spawn = availableSpawns[spawnIndex];
            availableSpawns.RemoveAt(spawnIndex);

            GameObject prefab;

            if (spawnBosses)
            {
                prefab = RoomManager.Instance.miniBossPrefabs[
                    Random.Range(0, RoomManager.Instance.miniBossPrefabs.Length)
                ];
            }
            else
            {
                if (enemyPrefabs.Count == 0)
                    return;

                prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            }

            GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.parentRoom = this;

            spawnedEnemies.Add(enemy);

            EnemySpawnEvents.EnemySpawned(enemy);   // EVENT HOOK

            IBoss[] bosses = enemy.GetComponents<IBoss>();
            foreach (IBoss boss in bosses)
                boss.WakeUp();
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

    public void ApplyBossDoorSprites()
    {
        useBossDoorSprites = true;

        if (hasTopDoor && topDoor != null)
            topDoor.GetComponent<SpriteRenderer>().sprite = bossTopDoorSprite;

        if (hasBottomDoor && bottomDoor != null)
            bottomDoor.GetComponent<SpriteRenderer>().sprite = bossBottomDoorSprite;

        if (hasLeftDoor && leftDoor != null)
            leftDoor.GetComponent<SpriteRenderer>().sprite = bossLeftDoorSprite;

        if (hasRightDoor && rightDoor != null)
            rightDoor.GetComponent<SpriteRenderer>().sprite = bossRightDoorSprite;

        if (hasTopDoor && topClosedDoor != null)
            topClosedDoor.GetComponent<SpriteRenderer>().sprite = bossTopClosedSprite;

        if (hasBottomDoor && bottomClosedDoor != null)
            bottomClosedDoor.GetComponent<SpriteRenderer>().sprite = bossBottomClosedSprite;

        if (hasLeftDoor && leftClosedDoor != null)
            leftClosedDoor.GetComponent<SpriteRenderer>().sprite = bossLeftClosedSprite;

        if (hasRightDoor && rightClosedDoor != null)
            rightClosedDoor.GetComponent<SpriteRenderer>().sprite = bossRightClosedSprite;
    }

    private void SpawnBossReward()
    {
        if (!isBossRoom)
            return;

        var rewards = RoomManager.Instance.bossRewardItemPrefabs;
        if (rewards == null || rewards.Length == 0)
            return;

        // Filter out items already used this run
        var available = rewards
            .Where(r => !RoomManager.bossRewardsUsedThisRun.Contains(r))
            .ToList();

        // If no items left, stop — all collected this run
        if (available.Count == 0)
            return;

        // Pick random unused reward
        GameObject rewardPrefab = available[Random.Range(0, available.Count)];

        // Base position = trapdoor
        Vector3 basePos = trapdoorClosed != null
            ? trapdoorClosed.transform.position
            : transform.position;

        // Move it down manually
        Vector3 spawnPos = basePos + new Vector3(0f, -1.5f, 0f);

        Instantiate(rewardPrefab, spawnPos, Quaternion.identity, transform);

        // Mark as used this run
        RoomManager.bossRewardsUsedThisRun.Add(rewardPrefab);
    }

    private void SpawnDecor()
    {
        SpawnDecorGroup(floorDecorPoints, floorDecorPrefabs, 0.45f);
        SpawnDecorGroup(topWallDecorPoints, topWallDecorPrefabs, 0.30f);
        SpawnDecorGroup(bottomWallDecorPoints, bottomWallDecorPrefabs, 0.30f);
        SpawnDecorGroup(leftWallDecorPoints, leftWallDecorPrefabs, 0.30f);
        SpawnDecorGroup(rightWallDecorPoints, rightWallDecorPrefabs, 0.30f);
    }

    private void SpawnDecorGroup(List<Transform> points, List<GameObject> prefabs, float chance)
    {
        if (points == null || prefabs == null || prefabs.Count == 0)
            return;

        foreach (Transform point in points)
        {
            if (Random.value < chance)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
                Instantiate(prefab, point.position, Quaternion.identity, transform);
            }
        }
    }

}
