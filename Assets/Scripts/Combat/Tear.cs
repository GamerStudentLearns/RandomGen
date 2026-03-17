using UnityEngine;

public class Tear : MonoBehaviour
{
    public float damage;
    public float speed;
    public float range;
    public ParticleSystem destroyEffect;

    [Header("Sound Effects")]
    public AudioClip[] destroySounds;   // Assign multiple clips in Inspector
    public AudioSource audioSource;     // Assign in Inspector (on tear or child)

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
        // Spawn particles
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        // Play random sound
        PlayRandomDestroySound();

        Destroy(gameObject);
    }

    private void PlayRandomDestroySound()
    {
        if (audioSource == null || destroySounds == null || destroySounds.Length == 0)
            return;

        int index = Random.Range(0, destroySounds.Length);
        audioSource.PlayOneShot(destroySounds[index]);
    }
}
