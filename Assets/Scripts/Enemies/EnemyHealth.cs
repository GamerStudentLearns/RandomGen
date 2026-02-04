using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    private float currentHealth;

    [HideInInspector] public Room parentRoom;

    public float CurrentHealth => currentHealth;

    void Awake()
    {
        // Get floor number from RunManager
        int floor = RunManager.instance != null ? RunManager.instance.currentFloor : 1;

        // Floor 1 → +0
        // Floor 2 → +3
        // Floor 3 → +6
        // Floor 4 → +9
        maxHealth += (floor - 1) * 2;

        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (parentRoom != null && parentRoom.isBossRoom)
            BossHealthUI.instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }


    void Die()
    {
        if (HitStopController.instance != null)
            HitStopController.instance.Stop(0.03f);

        if (parentRoom != null && parentRoom.hasBoss)
            parentRoom.bossObject = null;
        if (parentRoom != null && parentRoom.isBossRoom)
            BossHealthUI.instance.Hide();


        Destroy(gameObject);
    }

}
