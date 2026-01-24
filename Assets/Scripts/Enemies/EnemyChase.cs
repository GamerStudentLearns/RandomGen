using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Startup Delay")]
    public float startDelay = 0.4f;
    private float delayTimer;

    public float moveSpeed = 2f;
    public float separationRadius = 0.6f;
    public float separationStrength = 1.5f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        delayTimer = startDelay;
    }

    void Update()
    {
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        Vector2 separation = Vector2.zero;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var col in nearby)
        {
            if (col.gameObject == gameObject) continue;
            if (!col.CompareTag("Enemy")) continue;

            Vector2 diff = (Vector2)(transform.position - col.transform.position);
            separation += diff.normalized / diff.magnitude;
        }

        Vector2 finalDirection = (directionToPlayer + separation * separationStrength).normalized;
        transform.position += (Vector3)(finalDirection * moveSpeed * Time.deltaTime);
    }
}
