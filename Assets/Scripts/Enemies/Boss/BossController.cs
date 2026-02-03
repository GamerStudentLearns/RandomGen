using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth health;
    public Transform firePoint;
    public GameObject projectilePrefab;
    private Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Shooting")]
    public float shootCooldown = 1.5f;
    private float shootTimer;

    void Start()
    {
        if (health == null)
            health = GetComponent<EnemyHealth>();

        // Auto-find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        MoveTowardPlayer();
        HandleShooting();
    }

    void MoveTowardPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer > 0) return;

        shootTimer = shootCooldown;
        ShootAtPlayer();
    }

    void ShootAtPlayer()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();

        p.damagesPlayer = true;
        p.damagesEnemies = false;
        p.SetDirection(dir);
    }
}
