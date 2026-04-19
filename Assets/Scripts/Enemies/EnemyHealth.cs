using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Progression Unlocks")]
    public bool unlocksLevel6 = false;          // Your old Level 6 unlock
    public bool unlocksSpecialButton = false;   // Your old Special Button unlock

    [System.Serializable]
    public class BossUnlockFlag
    {
        public string unlockID;   // e.g. "LorePage"
        public bool unlocksThis;  // tick in inspector
    }

    [Header("Lore Unlocks (Slot-Specific)")]
    public BossUnlockFlag[] bossUnlocks;        // NEW: This is where you put "LorePage"

    [Header("Health Settings")]
    public float maxHealth = 5f;
    private float currentHealth;

    [Header("Boss Settings")]
    public bool isBoss = false;   // Toggle for boss scaling + UI

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
        // Hitstop effect
        if (HitStopController.instance != null)
            HitStopController.instance.Stop(0.03f);

        // If this enemy was the boss, clear the reference
        if (parentRoom != null && parentRoom.hasBoss)
            parentRoom.bossObject = null;

        // Hide boss health UI if this was a boss
        if (parentRoom != null && parentRoom.isBossRoom)
            BossHealthUI.instance.Hide();

        // -------------------------
        // SLOT-SPECIFIC LORE UNLOCKS
        // -------------------------
        HandleBossLoreUnlocks();

        // -------------------------
        // OLD UNLOCKS (PER SLOT)
        // -------------------------
        if (unlocksLevel6)
            SaveManager.SetLevel6Cleared();

        if (unlocksSpecialButton)
            SaveManager.SetSpecialButtonUnlocked();

        // Destroy the enemy object
        Destroy(gameObject);
    }

    private void HandleBossLoreUnlocks()
    {
        foreach (var flag in bossUnlocks)
        {
            if (!flag.unlocksThis || string.IsNullOrEmpty(flag.unlockID))
                continue;

            // Convert "LorePage" → "LorePage_Slot1/2/3"
            string slotSpecificID = SaveManager.GetSlotSpecificUnlock(flag.unlockID);

            // Save per-slot unlock
            SaveManager.SetBossUnlock(slotSpecificID);
        }
    }
}
