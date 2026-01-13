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
        // Enemy bullet hits player
        if (damagesPlayer && other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // Player bullet hits enemy
        if (damagesEnemies && other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // Destroy on walls
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
