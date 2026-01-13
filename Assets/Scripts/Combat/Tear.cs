using UnityEngine;
public class Tear : MonoBehaviour
{
    public float damage;
    public float speed;
    public float range;
    private Vector2 startPos;
    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        if (Vector2.Distance(startPos, transform.position) > range)
            Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}