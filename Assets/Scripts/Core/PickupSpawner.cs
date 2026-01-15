using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Range(0f, 1f)]
    public float spawnChance = 0.25f;   // 25% chance by default

    public GameObject[] possiblePickups; // health, coins, keys, etc.

    [Header("Spawn Position")]
    public Transform spawnPoint; // optional, defaults to room center

    private bool hasSpawned = false;

    public void TrySpawnPickup()
    {
        if (hasSpawned) return; // only spawn once per room
        hasSpawned = true;

        if (possiblePickups.Length == 0) return;

        float roll = Random.value;

        if (roll <= spawnChance)
        {
            GameObject pickup = possiblePickups[Random.Range(0, possiblePickups.Length)];

            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;

            Instantiate(pickup, pos, Quaternion.identity);
        }
    }
}
