using System;
using UnityEngine;

public static class EnemySpawnEvents
{
    public static Action<GameObject> OnEnemySpawned;

    public static void EnemySpawned(GameObject enemy)
        => OnEnemySpawned?.Invoke(enemy);
}
