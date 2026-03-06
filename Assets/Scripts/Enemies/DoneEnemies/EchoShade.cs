using UnityEngine;

public class EchoShade : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Combat")]
    public float fireInterval = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private Vector2 lastPlayerPos;
    private float fireTimer;
    private Vector2 oppositeMoveDir;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            lastPlayerPos = player.position;

        fireTimer = fireInterval;
    }

    void Update()
    {
        if (!player) return;

        UpdateOppositeMovement();
        HandleShooting();
    }

    void UpdateOppositeMovement()
    {
        Vector2 currentPlayerPos = player.position;
        Vector2 playerMoveDir = currentPlayerPos - lastPlayerPos;
        lastPlayerPos = currentPlayerPos;

        if (playerMoveDir.sqrMagnitude > 0.0001f)
            oppositeMoveDir = -playerMoveDir.normalized;

        transform.position += (Vector3)(oppositeMoveDir * moveSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            ShootAtPlayer();
            fireTimer = fireInterval;
        }
    }

    void ShootAtPlayer()
    {
        if (!projectilePrefab || !firePoint || !player) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetDirection(dir);
    }
}
