using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;

    public Room parentRoom;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Notify room (optional)
        if (parentRoom != null)
            parentRoom.bossObject = null;

        Destroy(gameObject);
    }
}
