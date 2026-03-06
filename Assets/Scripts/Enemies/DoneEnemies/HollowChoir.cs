using UnityEngine;

public class HollowChoir : MonoBehaviour
{
    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireInterval = 1.5f;

    private static int choirCount = 0;
    private float fireTimer;
    private Transform player;

    void OnEnable()
    {
        choirCount++;
    }

    void OnDisable()
    {
        choirCount = Mathf.Max(0, choirCount - 1);
    }

    void Start()
    {
        fireTimer = baseFireInterval;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FirePattern();
            fireTimer = Mathf.Max(0.4f, baseFireInterval - 0.2f * (choirCount - 1));
        }
    }

    void FirePattern()
    {
        if (!projectilePrefab || !firePoint || !player) return;

        // Base direction toward the player
        Vector2 baseDir = (player.position - firePoint.position).normalized;

        int patternLevel = Mathf.Clamp(choirCount, 1, 3);

        if (patternLevel == 1)
        {
            // Single shot aimed at player
            SpawnProjectile(baseDir);
        }
        else if (patternLevel == 2)
        {
            // 3‑way spread centered on the player
            SpawnProjectile(Rotate(baseDir, -15f));
            SpawnProjectile(baseDir);
            SpawnProjectile(Rotate(baseDir, 15f));
        }
        else
        {
            // 6‑way burst around the player direction
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f;
                SpawnProjectile(Rotate(baseDir, angle));
            }
        }
    }

    void SpawnProjectile(Vector2 dir)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetDirection(dir);
    }

    // Rotate a vector by degrees
    Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
