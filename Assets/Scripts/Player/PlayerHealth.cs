using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private const int TOTAL_HEART_LIMIT = 12;

    [Header("Health Settings")]
    public int maxHearts = 6;       // red heart containers
    public int currentHearts = 6;   // filled red hearts
    public int soulHearts = 0;      // blue hearts

    [Header("UI")]
    public HeartUI heartUI;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    [Header("Invulnerability")]
    public float invulnTime = 1f;
    private bool invulnerable;

    [Header("Damage Flash")]
    private Renderer[] renderers;
    private Color[] originalColors;
    public float flashDuration = 0.1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip damageSound;

    void Awake()
    {
        LoadFromRunManager();
        CacheRenderers();
        UpdateUI();
    }

    // -----------------------------
    //          DAMAGE
    // -----------------------------
    public void TakeDamage(int dmg)
    {
        if (invulnerable)
            return;

        PlayDamageSound();

        // Soul hearts absorb first
        if (soulHearts > 0)
        {
            soulHearts -= dmg;

            // Overflow hits red hearts
            if (soulHearts < 0)
            {
                currentHearts += soulHearts; // soulHearts is negative
                soulHearts = 0;
            }
        }
        else
        {
            currentHearts -= dmg;
        }

        NormalizeHearts();
        UpdateUI();

        StartCoroutine(FlashRed());
        StartCoroutine(Invulnerability());

        if (currentHearts <= 0)
            Die();
    }

    // -----------------------------
    //      SOUL HEART PICKUP
    // -----------------------------
    public bool CanPickUpSoulHeart()
    {
        return (currentHearts + soulHearts) < TOTAL_HEART_LIMIT;
    }

    public void AddSoulHearts(int amount)
    {
        soulHearts += amount;
        NormalizeHearts();
        UpdateUI();
    }

    // -----------------------------
    //      HEART CONTAINER PICKUP
    // -----------------------------
    public void AddHeartContainer(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int total = currentHearts + soulHearts;

            // If full (12), swap soul → red
            if (total >= TOTAL_HEART_LIMIT)
            {
                if (soulHearts > 0)
                {
                    soulHearts--;
                    maxHearts++;
                    currentHearts = Mathf.Min(currentHearts + 1, maxHearts);
                }
                continue;
            }

            // Normal container pickup
            maxHearts++;
            currentHearts = Mathf.Min(currentHearts + 1, maxHearts);
        }

        NormalizeHearts();
        UpdateUI();
    }

    // -----------------------------
    //             HEAL
    // -----------------------------
    public void Heal(int amount)
    {
        currentHearts += amount;
        NormalizeHearts();
        UpdateUI();
    }

    // -----------------------------
    //       NORMALIZATION
    // -----------------------------
    private void NormalizeHearts()
    {
        // Clamp red hearts to container size
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);

        // Clamp total to 12
        int total = currentHearts + soulHearts;
        if (total > TOTAL_HEART_LIMIT)
            soulHearts = TOTAL_HEART_LIMIT - currentHearts;

        // No negative soul hearts
        soulHearts = Mathf.Max(0, soulHearts);

        SaveToRunManager();
    }

    // -----------------------------
    //             UI
    // -----------------------------
    private void UpdateUI()
    {
        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);
    }

    // -----------------------------
    //         UTILITIES
    // -----------------------------
    private void PlayDamageSound()
    {
        bool enabled = PlayerPrefs.GetInt("DamageSoundEnabled", 1) == 1;
        if (enabled && audioSource && damageSound)
            audioSource.PlayOneShot(damageSound);
    }

    private void CacheRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    private IEnumerator FlashRed()
    {
        foreach (var r in renderers)
            r.material.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
    }

    private void Die()
    {
        Time.timeScale = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Destroy(gameObject);
    }

    // -----------------------------
    //         SAVE / LOAD
    // -----------------------------
    private void LoadFromRunManager()
    {
        if (RunManager.instance == null)
            new GameObject("RunManager").AddComponent<RunManager>();

        maxHearts = RunManager.instance.MaxHearts;
        currentHearts = RunManager.instance.currentHearts;
        soulHearts = RunManager.instance.soulHearts;

        NormalizeHearts();
    }

    private void SaveToRunManager()
    {
        RunManager.instance.currentHearts = currentHearts;
        RunManager.instance.soulHearts = soulHearts;

        // FIXED: use baseMaxHearts, not baseHearts
        RunManager.instance.heartModifiers = maxHearts - RunManager.instance.baseMaxHearts;
    }
}
