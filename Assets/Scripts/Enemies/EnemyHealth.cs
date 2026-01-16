using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    [HideInInspector] public float CurrentHealth;

    [HideInInspector] public Room parentRoom;

    void Awake() => CurrentHealth = maxHealth;

    public event System.Action<float, float> OnHealthChanged;

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0) Die();
    }


    void Die()
    {
        if (HitStopController.instance != null)
            HitStopController.instance.Stop(0.03f);

        Destroy(gameObject);
    }
}
