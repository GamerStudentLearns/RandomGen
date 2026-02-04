using UnityEngine;

public class HolyBeamDamage : MonoBehaviour
{
    public int damage = 1;
    public float duration = 0.5f;   // how long the beam exists
    public float tickRate = 0.1f;   // how often it deals damage while touching

    private float timer;
    private float tickTimer;

    private void Start()
    {
        timer = duration;
        tickTimer = 0f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        tickTimer -= Time.deltaTime;

        if (timer <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (tickTimer > 0) return;

        if (other.CompareTag("Player"))
        {
            // Try to get a health component
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            tickTimer = tickRate; // reset damage tick
        }
    }
}
