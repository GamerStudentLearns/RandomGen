using UnityEngine;

public class PulseWaveBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float moveSpeed = 1.2f;
    public float pulseCooldown = 3f;
    private float pulseTimer;

    public GameObject projectilePrefab;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        pulseTimer = pulseCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;

        pulseTimer -= Time.deltaTime;
        if (pulseTimer <= 0)
        {
            EmitPulse();
            pulseTimer = pulseCooldown;
        }
    }

    private void EmitPulse()
    {
        int count = 16;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * step;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 4f;
        }
    }

    public void WakeUp() => isAwake = true;
}
