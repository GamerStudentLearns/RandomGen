using UnityEngine;

public class GravityWellBoss : MonoBehaviour, IBoss
{
    [Header("References")]
    public Transform player;

    [Header("State")]
    public bool isAwake = false;

    [Header("Gravity Pull")]
    public float pullStrength = 3f;
    public float pullDuration = 1.2f;
    public float cooldown = 3f;

    private float timer;
    private bool pulling;

    [Header("Burst Attack")]
    public GameObject projectilePrefab;
    public int burstCount = 16;
    public float projectileSpeed = 7f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float directionChangeInterval = 1.5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float moveTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        timer = cooldown;
        PickNewMoveDirection();
    }

    private void Update()
    {
        if (!isAwake || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        HandleMovement();
        HandleGravityCycle();
    }

    // -------------------------
    // MOVEMENT
    // -------------------------
    private void HandleMovement()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0f)
            PickNewMoveDirection();

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void PickNewMoveDirection()
    {
        moveTimer = directionChangeInterval;
        moveDirection = Random.insideUnitCircle.normalized;
    }

    // -------------------------
    // GRAVITY + BURST LOGIC
    // -------------------------
    private void HandleGravityCycle()
    {
        timer -= Time.deltaTime;

        if (pulling)
        {
            PullPlayer();

            if (timer <= 0)
            {
                pulling = false;
                ShootBurst();
                timer = cooldown;
            }
        }
        else
        {
            if (timer <= 0)
            {
                pulling = true;
                timer = pullDuration;
            }
        }
    }

    private void PullPlayer()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        player.position += (Vector3)dir * pullStrength * Time.deltaTime;
    }

    private void ShootBurst()
    {
        float step = 360f / burstCount;

        for (int i = 0; i < burstCount; i++)
        {
            float angle = i * step;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity =
                rot * Vector2.right * projectileSpeed;
        }
    }

    public void WakeUp()
    {
        isAwake = true;
        timer = cooldown;
    }
}
