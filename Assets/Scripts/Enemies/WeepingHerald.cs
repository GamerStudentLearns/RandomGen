using UnityEngine;

public class WeepingHerald : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;

    [Header("Buff")]
    public float buffRadius = 4f;
    public float buffAmount = 2f;
    public float buffInterval = 1.5f;

    private Transform player;
    private float buffTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        buffTimer = buffInterval;
    }

    void Update()
    {
        MoveTowardsPlayer();
        HandleBuff();
    }

    void MoveTowardsPlayer()
    {
        if (!player) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }

    void HandleBuff()
    {
        buffTimer -= Time.deltaTime;
        if (buffTimer <= 0f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, buffRadius);
            foreach (var hit in hits)
            {
                if (!hit || hit.gameObject == gameObject) continue;
                if (hit.CompareTag("Enemy"))
                {
                    EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                    if (eh != null)
                    {
                        eh.maxHealth += buffAmount;
                    }
                }
            }

            buffTimer = buffInterval;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }
}
