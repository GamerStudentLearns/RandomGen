using UnityEngine;

public class ButtersBoss : MonoBehaviour
{
    public Transform player;
    public GameObject tearPrefab;

    [Header("Movement")]
    public float moveSpeed = 1.5f;   // slower than Margarine

    [Header("Attack Settings")]
    public float fireCooldown = 1.2f;
    public int fanCount = 5;
    public float angleStep = 12f;
    public float bulletSpeed = 6f;

    private float fireTimer;

    [HideInInspector] public bool isAwake = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        fireTimer = fireCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null)
            return;

        MoveTowardPlayer();
        HandleShooting();
    }

    private void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            FireFan();
            fireTimer = fireCooldown;
        }
    }

    private void FireFan()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        int half = fanCount / 2;

        for (int i = -half; i <= half; i++)
        {
            float angle = baseAngle + (i * angleStep);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 shotDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject t = Instantiate(tearPrefab, transform.position, Quaternion.identity);
            t.GetComponent<Rigidbody2D>().linearVelocity = shotDir * bulletSpeed;
        }
    }
}
