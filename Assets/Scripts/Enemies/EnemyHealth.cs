using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    private float currentHealth;

    [Header("Boss Settings")]
    public bool isBoss = false;   // NEW TOGGLE

    [HideInInspector] public Room parentRoom;

    public float CurrentHealth => currentHealth;

    // FLASH FIELDS
    private Renderer[] renderers;
    private Color[] originalColors;
    public float flashDuration = 0.1f;

    void Awake()
    {
        int floor = RunManager.instance != null ? RunManager.instance.currentFloor : 1;

        // --------------------
        // FLOOR SCALING
        // --------------------
        if (isBoss)
        {
            // Boss scaling: +15 per floor after floor 1
            maxHealth += (floor - 1) * 15f;
        }
        else
        {
            // Normal enemy scaling: +2 per floor after floor 1
            maxHealth += (floor - 1) * 2f;
        }

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

        StartCoroutine(FlashRed());

        if (parentRoom != null && parentRoom.isBossRoom)
            BossHealthUI.instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private System.Collections.IEnumerator FlashRed()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
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
