using UnityEngine;

public class TumorPod : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject clotProjectilePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2.5f;

    [Header("Homing")]
    public float homingDuration = 2f;

    private float spawnTimer;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnClot();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnClot()
    {
        if (!clotProjectilePrefab || !spawnPoint || !player) return;

        GameObject proj = Instantiate(clotProjectilePrefab, spawnPoint.position, Quaternion.identity);
        HomingProjectile homing = proj.AddComponent<HomingProjectile>();
        homing.target = player;
        homing.homingTime = homingDuration;
    }
}



