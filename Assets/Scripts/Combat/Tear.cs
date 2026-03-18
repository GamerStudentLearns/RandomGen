using UnityEngine;

public class Tear : MonoBehaviour
{
    public float damage;
    public float speed;
    public float range;
    public ParticleSystem destroyEffect;

    [Header("Sound")]
    public AudioClip[] destroySounds;   // Random clip chosen from here
    [Range(0f, 1f)]
    public float playChance = 0.5f;     // 50/50 chance by default
    public float volume = 1f;

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
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
            PlayDestroyEffectAndDie();
            return;
        }

        if (other.TryGetComponent(out FighterHitbox hitbox))
        {
            hitbox.TakeDamage(damage);
            PlayDestroyEffectAndDie();
            return;
        }

        if (other.CompareTag("Wall"))
        {
            PlayDestroyEffectAndDie();
        }
    }

    private void PlayDestroyEffectAndDie()
    {
        // Particle effect
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        // Sound effect (random chance + random clip)
        PlayRandomDestroySound();

        Destroy(gameObject);
    }

    private void PlayRandomDestroySound()
    {
        // Chance check
        if (Random.value > playChance)
            return;

        // Safety checks
        if (destroySounds == null || destroySounds.Length == 0)
            return;

        // Pick random clip
        int index = Random.Range(0, destroySounds.Length);
        AudioClip clip = destroySounds[index];

        // Play at the tear's position (survives object destruction)
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}
