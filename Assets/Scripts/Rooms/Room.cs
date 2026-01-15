using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [Header("Doors (used by RoomManager)")]
    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;


    [Header("Room Settings")]
    public bool isStartingRoom = false;

    [Header("Objects to toggle")]
    public GameObject[] objectsToDisableOnEnter;   // Doors, blockers, etc.
    public GameObject[] objectsToEnableOnClear;    // Usually the same objects

    [Header("Enemy Spawning")]
    public List<Transform> enemySpawnPoints;
    public List<GameObject> enemyPrefabs;

    public Vector2Int RoomIndex { get; set; }

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool roomActivated = false;

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


    // ---------------------------------------------------------
    // PLAYER ENTERS ROOM
    // ---------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (roomActivated) return;
        if (!other.CompareTag("Player")) return;

        roomActivated = true;

        // Skip combat for starting room
        if (isStartingRoom)
            return;

        // Disable doors/blockers
        foreach (var obj in objectsToDisableOnEnter)
            if (obj != null) obj.SetActive(false);

        SpawnEnemies();
        StartCoroutine(CheckRoomClear());
    }

    // ---------------------------------------------------------
    // ENEMY SPAWNING
    // ---------------------------------------------------------
    private void SpawnEnemies()
    {
        if (enemyPrefabs.Count == 0 || enemySpawnPoints.Count == 0)
            return;

        // Make a temporary list so we don't reuse spawn points
        List<Transform> availableSpawns = new List<Transform>(enemySpawnPoints);

        int enemyCount = availableSpawns.Count; // one enemy per spawn point

        for (int i = 0; i < enemyCount; i++)
        {
            // Pick a random spawn point
            int spawnIndex = Random.Range(0, availableSpawns.Count);
            Transform spawn = availableSpawns[spawnIndex];
            availableSpawns.RemoveAt(spawnIndex);

            // Pick a random enemy
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Spawn it
            GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);

            // Assign parent room
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.parentRoom = this;

            spawnedEnemies.Add(enemy);
        }
    }


    // ---------------------------------------------------------
    // CHECK IF ROOM IS CLEARED
    // ---------------------------------------------------------
    private IEnumerator CheckRoomClear()
    {
        while (true)
        {
            spawnedEnemies.RemoveAll(e => e == null);

            if (spawnedEnemies.Count == 0)
            {
                // Reactivate doors/blockers
                foreach (var obj in objectsToEnableOnClear)
                    if (obj != null) obj.SetActive(true);

                yield break;
            }

            yield return null;
        }
    }
}
