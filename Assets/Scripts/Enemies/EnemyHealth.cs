using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    private float currentHealth;

    [HideInInspector] public Room parentRoom;

    public float CurrentHealth => currentHealth;

    // --------------------
    // FLASH FIELDS
    // --------------------
    private Renderer[] renderers;
    private Color[] originalColors;
    public float flashDuration = 0.1f;

    void Awake()
    {
        // Get floor number from RunManager
        int floor = RunManager.instance != null ? RunManager.instance.currentFloor : 1;

        // Floor scaling
        maxHealth += (floor - 1) * 2;
        currentHealth = maxHealth;

        // --------------------
        // CACHE RENDERERS FOR FLASH
        // --------------------
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // FLASH RED
        StartCoroutine(FlashRed());

        if (parentRoom != null && parentRoom.isBossRoom)
            BossHealthUI.instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    // --------------------
    // FLASH RED EFFECT
    // --------------------
    private System.Collections.IEnumerator FlashRed()
    {
        // Set all renderers to red
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        // Restore original colors
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
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
