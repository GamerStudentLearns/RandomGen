using UnityEngine;

public class WideFanBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject tearPrefab;

    public float sweepCooldown = 2f;
    private float sweepTimer;

    // Sweep offset relative to the player-facing direction
    private float sweepAngle = -60f;
    private bool sweepingRight = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        sweepTimer = sweepCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        sweepTimer -= Time.deltaTime;

        if (sweepTimer <= 0)
        {
            FireSweep();
            sweepTimer = sweepCooldown;
        }
    }

    private void FireSweep()
    {
        // Move sweep angle left/right
        if (sweepingRight)
        {
            sweepAngle += 8f;
            if (sweepAngle >= 60f) sweepingRight = false;
        }
        else
        {
            sweepAngle -= 8f;
            if (sweepAngle <= -60f) sweepingRight = true;
        }

        // Aim base direction at the player
        Vector2 dir = player.position - transform.position;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Add sweep offset
        float finalAngle = baseAngle + sweepAngle;

        Quaternion rot = Quaternion.Euler(0, 0, finalAngle);

        GameObject tear = Instantiate(tearPrefab, transform.position, rot);
        tear.GetComponent<Rigidbody2D>().linearVelocity =
            rot * Vector2.right * 4f;
    }

    public void WakeUp() => isAwake = true;
}
