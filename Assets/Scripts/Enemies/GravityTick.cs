using UnityEngine;

public class GravityTick : MonoBehaviour
{
    [Header("Pulse")]
    public float pulseInterval = 3f;
    public float pulseDuration = 0.8f;
    public float pullStrength = 3f;
    public float pullRadius = 4f;

    private float pulseTimer;
    private float activeTimer;
    private bool pulsing;

    void Start()
    {
        pulseTimer = pulseInterval;
    }

    void Update()
    {
        if (!pulsing)
        {
            pulseTimer -= Time.deltaTime;
            if (pulseTimer <= 0f)
            {
                StartPulse();
            }
        }
        else
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f)
            {
                EndPulse();
            }
            else
            {
                ApplyPull();
            }
        }
    }

    void StartPulse()
    {
        pulsing = true;
        activeTimer = pulseDuration;
    }

    void EndPulse()
    {
        pulsing = false;
        pulseTimer = pulseInterval;
    }

    void ApplyPull()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (var hit in hits)
        {
            if (!hit) continue;

            if (hit.CompareTag("Player") || hit.GetComponent<Projectile>())
            {
                Vector2 dir = (Vector2)transform.position - (Vector2)hit.transform.position;
                hit.transform.position += (Vector3)(dir.normalized * pullStrength * Time.deltaTime);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
