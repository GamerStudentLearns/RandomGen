using UnityEngine;

public class WideFanBoss : MonoBehaviour, IBoss
{
    [Header("References")]
    public Transform player;
    public GameObject tearPrefab;

    [Header("Attack Settings")]
    public float attackCooldown = 1.2f;   // Faster attacks
    public float projectileSpeed = 6f;
    public float fanAngle = 20f;          // Spread between bullets
    public int shotsPerBurst = 3;         // How many fan shots per attack
    public float burstSpacing = 0.12f;    // Time between shots in the burst

    private float attackTimer;
    private bool isFiringBurst = false;

    public bool isAwake = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null)
            return;

        HandleAttacks();
    }

    // -------------------------
    // ATTACKS ONLY
    // -------------------------
    private void HandleAttacks()
    {
        if (isFiringBurst) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            StartCoroutine(FireBurst());
            attackTimer = attackCooldown;
        }
    }

    private System.Collections.IEnumerator FireBurst()
    {
        isFiringBurst = true;

        for (int i = 0; i < shotsPerBurst; i++)
        {
            FireFanShot();
            yield return new WaitForSeconds(burstSpacing);
        }

        isFiringBurst = false;
    }

    private void FireFanShot()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        FireSingleShot(baseAngle);                 // center
        FireSingleShot(baseAngle + fanAngle);      // right
        FireSingleShot(baseAngle - fanAngle);      // left
    }

    private void FireSingleShot(float angle)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject tear = Instantiate(tearPrefab, transform.position, rot);
        tear.GetComponent<Rigidbody2D>().linearVelocity =
            rot * Vector2.right * projectileSpeed;
    }

    public void WakeUp() => isAwake = true;
}
