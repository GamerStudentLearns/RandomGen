using UnityEngine;

public class FlySpawnerBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject minionPrefab;
    public GameObject projectilePrefab;

    public float spawnCooldown = 1.5f;
    private float spawnTimer;

    public float shotCooldown = 3f;
    private float shotTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnTimer = spawnCooldown;
        shotTimer = shotCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        spawnTimer -= Time.deltaTime;
        shotTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnFly();
            spawnTimer = spawnCooldown;
        }

        if (shotTimer <= 0)
        {
            ShootAtPlayer();
            shotTimer = shotCooldown;
        }
    }

    private void SpawnFly()
    {
        Vector2 offset = Random.insideUnitCircle * 1.5f;
        Instantiate(minionPrefab, transform.position + (Vector3)offset, Quaternion.identity);
    }

    private void ShootAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 6f;
    }

    public void WakeUp() => isAwake = true;
}
