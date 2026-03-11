using UnityEngine;

public class Tear : MonoBehaviour
{
    public float damage;
    public float speed;
    public float range;
    public ParticleSystem destroyEffect;   // Assign in Inspector

    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector2.Distance(startPos, transform.position) > range)
            PlayDestroyEffectAndDie();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // -----------------------------
        // 1. Normal enemies / bosses
        // -----------------------------
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
            PlayDestroyEffectAndDie();
            return;
        }

        // -----------------------------
        // 2. DuoBoss fighters (Butters / Margarine)
        // -----------------------------
        if (other.TryGetComponent(out FighterHitbox hitbox))
        {
            hitbox.TakeDamage(damage);
            PlayDestroyEffectAndDie();
            return;
        }

        // -----------------------------
        // 3. Walls
        // -----------------------------
        if (other.CompareTag("Wall"))
        {
            PlayDestroyEffectAndDie();
        }
    }

    private void PlayDestroyEffectAndDie()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
