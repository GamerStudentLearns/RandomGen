using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 8f;
    public int damage = 1;
    public float lifeTime = 5f;

    [Header("Damage Targets")]
    public bool damagesPlayer;
    public bool damagesEnemies;

    [Header("Effects")]
    public ParticleSystem destroyEffect;

    [Header("Sound")]
    public AudioClip[] destroySounds;   // Random clip chosen from here
    [Range(0f, 1f)]
    public float playChance = 0.5f;     // 50% chance by default
    public float volume = 1f;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (damagesPlayer && other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(damage);

            PlayDestroyEffectAndDie();
            return;
        }

        if (damagesEnemies && other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                ProjectileEvents.PlayerProjectileHit(enemy);
            }

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

        // Sound effect
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

        // Play at projectile position (survives object destruction)
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}
