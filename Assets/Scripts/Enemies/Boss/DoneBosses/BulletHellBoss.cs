using UnityEngine;

public class BulletHellBoss : MonoBehaviour, IBoss
{
    [Header("Movement & Player")]
    public float moveSpeed = 1.2f;
    public Transform player;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Boss State")]
    public bool isAwake = false;

    [Header("Attack Timers")]
    public float patternSwitchTime = 6f;
    private float patternTimer;

    public float fireRate = 0.1f;
    private float fireTimer;

    private int currentPattern = 0;
    private float angleOffset = 0f;

    // ---------------------------------------------------------
    // NEW — SPRITE FLAP ANIMATION
    // ---------------------------------------------------------
    [Header("Flap Animation")]
    public Sprite idleSprite;
    public Sprite flapSprite;
    public float flapSpeed = 0.15f;

    private SpriteRenderer sr;
    private float flapTimer = 0f;
    private bool flapState = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        patternTimer = patternSwitchTime;
        fireTimer = fireRate;

        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        MoveTowardPlayer();
        HandlePatterns();
        AnimateFlap();
    }

    private void AnimateFlap()
    {
        flapTimer -= Time.deltaTime;

        if (flapTimer <= 0f)
        {
            flapState = !flapState;
            sr.sprite = flapState ? flapSprite : idleSprite;
            flapTimer = flapSpeed;
        }
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------
    private void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    // ---------------------------------------------------------
    // PATTERN HANDLING
    // ---------------------------------------------------------
    private void HandlePatterns()
    {
        patternTimer -= Time.deltaTime;
        fireTimer -= Time.deltaTime;

        if (patternTimer <= 0)
        {
            currentPattern = (currentPattern + 1) % 3;
            patternTimer = patternSwitchTime;
        }

        if (fireTimer <= 0)
        {
            switch (currentPattern)
            {
                case 0: SpiralPattern(); break;
                case 1: WaveRingPattern(); break;
                case 2: SweepingArcPattern(); break;
            }

            fireTimer = fireRate;
        }
    }

    // ---------------------------------------------------------
    // PATTERN 1 — SPIRAL STREAM
    // ---------------------------------------------------------
    private void SpiralPattern()
    {
        angleOffset += 10f;
        float angle = angleOffset;

        Quaternion rot = Quaternion.Euler(0, 0, angle);
        GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
        proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 6f;
    }

    // ---------------------------------------------------------
    // PATTERN 2 — WAVE RINGS
    // ---------------------------------------------------------
    private void WaveRingPattern()
    {
        int count = 12;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * step + angleOffset;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 4f;
        }

        angleOffset += 15f;
    }

    // ---------------------------------------------------------
    // PATTERN 3 — SWEEPING ARC
    // ---------------------------------------------------------
    private float sweepAngle = -60f;
    private bool sweepRight = true;

    private void SweepingArcPattern()
    {
        if (sweepRight)
        {
            sweepAngle += 4f;
            if (sweepAngle >= 60f) sweepRight = false;
        }
        else
        {
            sweepAngle -= 4f;
            if (sweepAngle <= -60f) sweepRight = true;
        }

        Quaternion rot = Quaternion.Euler(0, 0, sweepAngle);
        GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
        proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 7f;
    }

    public void WakeUp()
    {
        isAwake = true;
        patternTimer = patternSwitchTime;
        fireTimer = fireRate;
    }
}
