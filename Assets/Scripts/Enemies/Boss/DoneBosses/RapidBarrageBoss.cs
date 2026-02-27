using UnityEngine;

public class RapidBarrageBoss : MonoBehaviour, IBoss
{
    [Header("Player Tracking")]
    public Transform player;
    public bool isAwake = false;

    [Header("Projectile")]
    public GameObject tearPrefab;
    public float fireRate = 0.3f;          // slower default
    public float projectileSpeed = 6f;      // slower shots
    public float spreadAngle = 20f;         // randomness

    [Header("Burst Firing")]
    public float burstDuration = 2f;        // time spent firing
    public float restDuration = 1.5f;       // time spent not firing
    private float cycleTimer;
    private bool isFiring = true;

    private float fireTimer;

    [Header("Movement")]
    public float moveSpeed = 1.5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        fireTimer = fireRate;
        cycleTimer = burstDuration;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        Move();
        UpdateCycle();

        if (isFiring)
            HandleFire();
    }

    private void Move()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void UpdateCycle()
    {
        cycleTimer -= Time.deltaTime;

        if (isFiring && cycleTimer <= 0)
        {
            isFiring = false;
            cycleTimer = restDuration;
        }
        else if (!isFiring && cycleTimer <= 0)
        {
            isFiring = true;
            cycleTimer = burstDuration;
        }
    }

    private void HandleFire()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0)
        {
            Vector2 baseDir = (player.position - transform.position).normalized;

            // Add random spread
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Vector2 finalDir = Quaternion.Euler(0, 0, angle) * baseDir;

            GameObject tear = Instantiate(tearPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = tear.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = finalDir * projectileSpeed;

            fireTimer = fireRate;
        }
    }

    public void WakeUp() => isAwake = true;
}
