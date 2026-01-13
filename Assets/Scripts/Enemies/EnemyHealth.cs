using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 5f;
    private float currentHealth;

   

    void Awake()
    {
        currentHealth = maxHealth;
    }

   

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Small Isaac-style hit stop
        if (HitStopController.instance != null)
            HitStopController.instance.Stop(0.03f);


        Destroy(gameObject);
    }
}
