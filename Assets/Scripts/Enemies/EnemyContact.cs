using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    public int contactDamage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(contactDamage);
            }
        }
    }
}
