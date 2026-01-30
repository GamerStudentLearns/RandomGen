using UnityEngine;

public class Burden : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 200f;
    public float moveSpeed = 0.5f;
    public float gravityStrength = 8f;
    public float pullRadius = 12f;

    [Header("Contact Damage")]
    public int contactDamage = 1;          // now an int
    public float contactCooldown = 0.75f;
    private float contactTimer = 0f;

    [Header("References")]
    public Transform player;
    public GameObject featherBurstPrefab;
    public Rigidbody2D rb;

    [Header("Optional")]
    public bool canMove = true;
    public float knockbackResistance = 0.9f;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        ApplyGravityPull();
        if (canMove) SlowCreepMovement();

        if (contactTimer > 0)
            contactTimer -= Time.fixedDeltaTime;
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------
    void SlowCreepMovement()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    // ---------------------------------------------------------
    // GRAVITY FIELD
    // ---------------------------------------------------------
    void ApplyGravityPull()
    {
        float dist = Vector2.Distance(player.position, transform.position);
        if (dist > pullRadius) return;

        Vector2 pullDir = (transform.position - player.position).normalized;
        float pullForce = gravityStrength * (1f - (dist / pullRadius));

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.AddForce(pullDir * pullForce, ForceMode2D.Force);
    }

    // ---------------------------------------------------------
    // CONTACT DAMAGE
    // ---------------------------------------------------------
    void OnCollisionStay2D(Collision2D collision)
    {
        if (contactTimer > 0) return;

        if (collision.collider.CompareTag("Player"))
        {
            var health = collision.collider.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(contactDamage); // now matches int signature
                contactTimer = contactCooldown;
            }
        }
    }

    // ---------------------------------------------------------
    // DAMAGE + DEATH
    // ---------------------------------------------------------
    public void TakeDamage(float amount, Vector2 knockback)
    {
        currentHealth -= amount;

        rb.AddForce(knockback * (1f - knockbackResistance), ForceMode2D.Impulse);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (featherBurstPrefab != null)
            Instantiate(featherBurstPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
