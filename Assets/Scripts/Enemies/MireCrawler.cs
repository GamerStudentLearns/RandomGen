using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class MireCrawler : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float wanderRadius = 2f;
    public float wanderInterval = 2.5f;
    public float moveThreshold = 0.05f;

    [Header("Splitting")]
    public bool canSplit = true;
    public int splitCount = 2;
    public GameObject smallerCrawlerPrefab;

    private Vector2 wanderTarget;
    private float wanderTimer;
    private EnemyHealth health;
    private bool hasSplit;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        PickNewWanderTarget();
    }

    void Update()
    {
        Wander();

        if (canSplit && !hasSplit && health.CurrentHealth <= 0)
        {
            Split();
            hasSplit = true;
        }
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

    void Split()
    {
        if (!smallerCrawlerPrefab) return;

        for (int i = 0; i < splitCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Instantiate(smallerCrawlerPrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }
    }
}
