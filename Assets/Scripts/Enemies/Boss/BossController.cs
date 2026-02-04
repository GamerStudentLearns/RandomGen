using UnityEngine;

public class BossController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform player;

    public float attackCooldown = 2f;
    private float attackTimer;

    public GameObject projectilePrefab;

    // NEW — boss starts asleep
    public bool isAwake = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (!isAwake) return;       // NEW — do nothing until awakened
        if (player == null) return;

        MoveTowardPlayer();
        HandleAttacks();
    }

    private void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            ShootRadial();
            attackTimer = attackCooldown;
        }
    }

    private void ShootRadial()
    {
        int count = 8;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 6f;
        }
    }

    // NEW — called by Room.cs when player enters
    public void WakeUp()
    {
        isAwake = true;
        attackTimer = attackCooldown; // reset attack timer so it doesn't fire instantly
    }
}
