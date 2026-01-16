using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    private Transform player;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    private float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        ChasePlayer();
        HandleShooting();
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            ShootAtPlayer();
            shootTimer = shootCooldown;
        }
    }

    void ShootAtPlayer()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().linearVelocity = direction * 6f; // adjust speed
    }
}
