using UnityEngine;

public class MireCrawler : MonoBehaviour
{
    [Header("Startup Delay")]
    public float startDelay = 0.7f;
    private float delayTimer;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float wanderRadius = 3f;
    public float wanderInterval = 2f;
    public float moveThreshold = 0.01f;

    [Header("Combat")]
    public float detectionRange = 6f;
    public float fireRate = 1.2f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;
    public Sprite mainSprite; // Only one sprite now

    private Transform player;
    private Vector2 wanderTarget;
    private float wanderTimer;
    private float fireTimer;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool isShooting;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PickNewWanderTarget();
        delayTimer = startDelay;

        // Set the single sprite
        if (spriteRenderer && mainSprite)
            spriteRenderer.sprite = mainSprite;
    }

    void Update()
    {
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isShooting = distanceToPlayer <= detectionRange;

        // Always move, even while shooting
        Wander();

        if (isShooting)
            AttackPlayer();
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
            PickNewWanderTarget();

        MoveTowards(wanderTarget);
    }

    void PickNewWanderTarget()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        wanderTimer = wanderInterval;
    }

    void AttackPlayer()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;
        proj.GetComponent<Projectile>().SetDirection(dir);
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 dir = target - (Vector2)transform.position;
        float distance = dir.magnitude;

        if (distance > moveThreshold)
        {
            Vector2 moveDir = dir.normalized;
            lastMoveDirection = moveDir;
            transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
        }
    }
}
