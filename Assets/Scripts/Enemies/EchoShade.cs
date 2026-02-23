using UnityEngine;

public class EchoShade : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Echo Logic")]
    public float echoDelay = 1f;

    [Header("Combat")]
    public float fireInterval = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private Vector2 lastPlayerDir;
    private Vector2 echoedDir;
    private float echoTimer;
    private float fireTimer;
    private Vector2 lastPlayerPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player) lastPlayerPos = player.position;
        echoTimer = echoDelay;
        fireTimer = fireInterval;
    }

    void Update()
    {
        if (!player) return;

        Vector2 currentPlayerPos = player.position;
        Vector2 playerMoveDir = (currentPlayerPos - lastPlayerPos).normalized;
        lastPlayerPos = currentPlayerPos;

        echoTimer -= Time.deltaTime;
        if (echoTimer <= 0f)
        {
            lastPlayerDir = playerMoveDir;
            echoedDir = lastPlayerDir;
            echoTimer = echoDelay;
        }

        MoveInEchoDirection();
        HandleShooting();
    }

    void MoveInEchoDirection()
    {
        if (echoedDir.sqrMagnitude < 0.01f) return;
        transform.position += (Vector3)(echoedDir * moveSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            if (projectilePrefab && firePoint && echoedDir.sqrMagnitude > 0.01f)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                proj.GetComponent<Projectile>().SetDirection(echoedDir);
            }
            fireTimer = fireInterval;
        }
    }
}
