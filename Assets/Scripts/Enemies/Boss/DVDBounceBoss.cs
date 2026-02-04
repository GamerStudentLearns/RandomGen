using UnityEngine;

public class DVDBounceBoss : MonoBehaviour, IBoss
{
    public float moveSpeed = 3f;
    public Vector2 moveDirection = new Vector2(1, 1); // normalized at Start

    public Transform player;
    public bool isAwake = false;

    public float attackCooldown = 2f;
    private float attackTimer;

    public GameObject projectilePrefab;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Normalize so speed is consistent regardless of initial direction
        moveDirection = moveDirection.normalized;

        attackTimer = attackCooldown;
    }

    private void Update()
    {
        if (!isAwake) return;

        Move();
        HandleAttacks();
    }

    private void Move()
    {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void HandleAttacks()
    {
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            ShootAtPlayer();
            attackTimer = attackCooldown;
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 6f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Wall")) return;

        // Reflect movement direction based on collision normal
        Vector2 normal = collision.contacts[0].normal;
        moveDirection = Vector2.Reflect(moveDirection, normal).normalized;
    }

    public void WakeUp()
    {
        isAwake = true;
        attackTimer = attackCooldown;
    }
}
