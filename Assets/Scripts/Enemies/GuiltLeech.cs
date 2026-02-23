using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class GuiltLeech : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public float stopDistance = 1.5f;

    [Header("Scaling")]
    public float damageBonusPerDeath = 0.5f;
    public float speedBonusPerDeath = 0.2f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireRate = 1.5f;

    private Transform player;
    private int deathsInRoom;
    private float fireTimer;
    private EnemyHealth health;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        health = GetComponent<EnemyHealth>();
        fireTimer = baseFireRate;
    }

    void Update()
    {
        if (!player) return;

        MoveTowardsPlayer();
        HandleShooting();
    }

    void MoveTowardsPlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            float currentSpeed = moveSpeed + deathsInRoom * speedBonusPerDeath;
            transform.position += (Vector3)(dir * currentSpeed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            if (projectilePrefab && firePoint && player)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Vector2 dir = (player.position - firePoint.position).normalized;
                Projectile p = proj.GetComponent<Projectile>();
                p.damage += Mathf.RoundToInt(deathsInRoom * damageBonusPerDeath);
                p.SetDirection(dir);
            }
            fireTimer = baseFireRate;
        }
    }

    // Call this from a room manager when any enemy dies in the same room
    public void NotifyEnemyDiedInRoom()
    {
        deathsInRoom++;
    }
}
