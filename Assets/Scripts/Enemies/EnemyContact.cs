using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    public int contactDamage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player == null)
            player = other.GetComponentInParent<PlayerHealth>();

        if (player != null)
            player.TakeDamage(contactDamage);
    }
}
