using UnityEngine;

public class HollowChoir : MonoBehaviour
{
    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireInterval = 1.5f;

    private static int choirCount = 0;
    private float fireTimer;

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
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FirePattern();
            fireTimer = Mathf.Max(0.4f, baseFireInterval - 0.2f * (choirCount - 1));
        }
    }

    void FirePattern()
    {
        if (!projectilePrefab || !firePoint) return;

        int patternLevel = Mathf.Clamp(choirCount, 1, 3);

        if (patternLevel == 1)
        {
            // Single forward shot (up)
            SpawnProjectile(Vector2.up);
        }
        else if (patternLevel == 2)
        {
            // Spread of 3
            SpawnProjectile(Quaternion.Euler(0, 0, -15) * Vector2.up);
            SpawnProjectile(Vector2.up);
            SpawnProjectile(Quaternion.Euler(0, 0, 15) * Vector2.up);
        }
        else
        {
            // Full 6-way burst
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                SpawnProjectile(dir);
            }
        }
    }

    void SpawnProjectile(Vector2 dir)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetDirection(dir);
    }
}
