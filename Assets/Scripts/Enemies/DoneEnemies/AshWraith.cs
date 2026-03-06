using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AshWraith : MonoBehaviour
{
    [Header("Startup Delay")]
    public float startDelay = 0.6f;
    private float delayTimer;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float wanderRadius = 3f;
    public float wanderInterval = 1.8f;
    public float moveThreshold = 0.05f;

    [Header("Vulnerability")]
    public float stillTimeToBeVulnerable = 0.25f;
    private float stillTimer;
    private bool isVulnerable;

    [Header("Combat")]
    public float fireRate = 1.2f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite movingSprite;
    public Sprite vulnerableSprite;
    public TrailRenderer fireTrail;

    private Vector2 wanderTarget;
    private float wanderTimer;
    private Vector2 lastPos;

    private Collider2D col;
    private Rigidbody2D rb;
    private Transform player;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        PickNewWanderTarget();
        delayTimer = startDelay;
        lastPos = transform.position;
        fireTimer = fireRate;
    }

    void Update()
    {
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        Wander();
        UpdateVulnerability();
        UpdateVisuals();
        HandleShooting();
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
            PickNewWanderTarget();

        Vector2 dir = wanderTarget - (Vector2)transform.position;
        float dist = dir.magnitude;

        if (dist > moveThreshold)
        {
            Vector2 moveDir = dir.normalized;
            Vector2 newPos = (Vector2)transform.position + moveDir * moveSpeed * Time.deltaTime;

            rb.MovePosition(newPos); // prevents wall clipping
        }
    }

    void PickNewWanderTarget()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        wanderTimer = wanderInterval;
    }

    void UpdateVulnerability()
    {
        float moved = Vector2.Distance(transform.position, lastPos);

        if (moved < 0.01f)
        {
            stillTimer += Time.deltaTime;
            if (stillTimer >= stillTimeToBeVulnerable)
                isVulnerable = true;
        }
        else
        {
            stillTimer = 0f;
            isVulnerable = false;
        }

        lastPos = transform.position;

        col.enabled = isVulnerable;
    }

    void HandleShooting()
    {
        if (isVulnerable) return; // only shoot when INVULNERABLE (moving)

        if (!player || !projectilePrefab || !firePoint) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Vector2 dir = (player.position - firePoint.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetDirection(dir);

            fireTimer = fireRate;
        }
    }

    void UpdateVisuals()
    {
        if (!spriteRenderer) return;
        spriteRenderer.sprite = isVulnerable ? vulnerableSprite : movingSprite;
    }
}
