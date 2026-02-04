using UnityEngine;

public class WideFanBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject tearPrefab;

    public float sweepCooldown = 2f;
    private float sweepTimer;

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
        // Sweep between -60 and +60 degrees
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

        Quaternion rot = Quaternion.Euler(0, 0, sweepAngle);
        GameObject tear = Instantiate(tearPrefab, transform.position, rot);
        tear.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 4f;
    }

    public void WakeUp() => isAwake = true;
}
