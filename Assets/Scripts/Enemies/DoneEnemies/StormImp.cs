using UnityEngine;

public class StormImp : MonoBehaviour
{
    [Header("Movement / Teleport")]
    public float teleportRadius = 3f;
    public float teleportInterval = 2f;
    public float startDelay = 0.5f;

    [Header("Combat")]
    public float detectionRange = 6f;
    public float fireDelayAfterTeleport = 0.2f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.2f;

    private Transform player;
    private float teleportTimer;
    private float fireTimer;
    private float delayTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        teleportTimer = teleportInterval;
        delayTimer = startDelay;
    }

    void Update()
    {
        if (!player) return;

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectionRange) return;

        teleportTimer -= Time.deltaTime;
        fireTimer -= Time.deltaTime;

        if (teleportTimer <= 0f)
        {
            TeleportAroundPlayer();
            teleportTimer = teleportInterval;
            fireTimer = fireDelayAfterTeleport;
        }

        if (fireTimer <= 0f)
        {
            ShootAtPlayer();
            fireTimer = fireRate;
        }
    }

    void TeleportAroundPlayer()
    {
        if (!player) return;

        // Choose one of 8 directions (45° increments)
        int index = Random.Range(0, 8);
        float angle = 45f * index * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * teleportRadius;

        transform.position = (Vector2)player.position + offset;
    }

    void ShootAtPlayer()
    {
        if (!projectilePrefab || !firePoint || !player) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;
        proj.GetComponent<Projectile>().SetDirection(dir);
    }
}
