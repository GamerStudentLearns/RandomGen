using UnityEngine;
using System.Collections.Generic;

public class RockSpawner : MonoBehaviour
{
    [Header("Rock Settings")]
    public GameObject rockPrefab;
    public int minRocks = 1;
    public int maxRocks = 4;

    [Range(0f, 1f)]
    public float spawnChance = 0.4f; // 40% chance a room gets rocks

    [Header("Room Bounds")]
    public float roomWidth = 20f;
    public float roomHeight = 12f;

    [Header("Avoid These Positions")]
    public List<Transform> forbiddenPoints; // player spawn points, doorways, etc.

    public void TrySpawnRocks()
    {
        // Roll the chance
        if (Random.value > spawnChance)
            return;

        int rockCount = Random.Range(minRocks, maxRocks + 1);

        for (int i = 0; i < rockCount; i++)
        {
            Vector3 pos = GetValidPosition();
            Instantiate(rockPrefab, pos, Quaternion.identity, transform);
        }
    }

    private Vector3 GetValidPosition()
    {
        Vector3 pos;

        int safety = 0;
        do
        {
            float x = Random.Range(-roomWidth / 2f + 1f, roomWidth / 2f - 1f);
            float y = Random.Range(-roomHeight / 2f + 1f, roomHeight / 2f - 1f);

            pos = transform.position + new Vector3(x, y, 0);

            safety++;
            if (safety > 50) break;

        } while (IsNearForbiddenPoint(pos));

        return pos;
    }

    private bool IsNearForbiddenPoint(Vector3 pos)
    {
        foreach (var point in forbiddenPoints)
        {
            if (Vector3.Distance(pos, point.position) < 2f)
                return true;
        }
        return false;
    }
}
