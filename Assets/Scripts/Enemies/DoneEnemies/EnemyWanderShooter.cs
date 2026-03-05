using UnityEngine;

public class EnemyWanderShooter : MonoBehaviour
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

    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite walkUp, walkDown, walkLeft, walkRight;
    public Sprite shootUp, shootDown, shootLeft, shootRight;

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

        if (isShooting)
        {
            AttackPlayer();
            UpdateShootingSprite();
        }
        else
        {
            Wander();
            UpdateWalkingSprite();
        }
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

    void UpdateWalkingSprite()
    {
        Vector2 dir = lastMoveDirection;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir = new Vector2(Mathf.Sign(dir.x), 0);
        else
            dir = new Vector2(0, Mathf.Sign(dir.y));

        if (dir.x != 0)
            spriteRenderer.sprite = dir.x > 0 ? walkRight : walkLeft;
        else
            spriteRenderer.sprite = dir.y > 0 ? walkUp : walkDown;
    }

    void UpdateShootingSprite()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            spriteRenderer.sprite = dir.x > 0 ? shootRight : shootLeft;
        else
            spriteRenderer.sprite = dir.y > 0 ? shootUp : shootDown;
    }
}
