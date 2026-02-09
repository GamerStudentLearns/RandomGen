using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Range(0f, 1f)]
    public float spawnChance = 0.25f;

    public GameObject[] possiblePickups;

    [Header("Spawn Positions")]
    public Transform spawnPoint;          // normal rooms
    public Transform bossSpawnPoint;      // top of room for boss rooms

    private bool hasSpawned = false;

    public void TrySpawnPickup(bool isBossRoom)
    {
        if (hasSpawned) return;
        hasSpawned = true;

        if (possiblePickups.Length == 0) return;

        float roll = Random.value;
        if (roll > spawnChance) return;

        GameObject pickup = possiblePickups[Random.Range(0, possiblePickups.Length)];

        Vector3 pos;

        if (isBossRoom && bossSpawnPoint != null)
            pos = bossSpawnPoint.position;
        else if (spawnPoint != null)
            pos = spawnPoint.position;
        else
            pos = transform.position;

        Instantiate(pickup, pos, Quaternion.identity);
    }
}
