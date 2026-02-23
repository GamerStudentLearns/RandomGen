using UnityEngine;

[RequireComponent(typeof(Collider2D))]
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

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite movingSprite;
    public Sprite vulnerableSprite;
    public TrailRenderer fireTrail;

    private Vector2 wanderTarget;
    private float wanderTimer;
    private Vector2 lastPos;
    private Collider2D col;

    void Start()
    {
        PickNewWanderTarget();
        delayTimer = startDelay;
        lastPos = transform.position;
        col = GetComponent<Collider2D>();
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
            transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
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

        // Collider only enabled when vulnerable
        if (col != null)
            col.enabled = isVulnerable;
    }

    void UpdateVisuals()
    {
        if (!spriteRenderer) return;
        spriteRenderer.sprite = isVulnerable ? vulnerableSprite : movingSprite;
    }
}
