using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 6;

    public GameObject deathEffect;
    public float invulnTime = 1f;

    [HideInInspector] public int currentHearts;
    private bool invulnerable;

    [Header("Soul Hearts")]
    public int soulHearts = 0;

    [Header("UI")]
    public HeartUI heartUI;

    [Header("Game Over UI")]
    public GameObject gameOverUI;   // ← Option 1 uses GameObject

    private PlayerStats stats;

    void Awake()
    {

        FindOrCreateHeartUI();
        // Ensure RunManager exists
        if (RunManager.instance == null)
        {
            var rmGo = new GameObject("RunManager");
            rmGo.AddComponent<RunManager>();
        }

        // Load stored health
        maxHearts = RunManager.instance.maxHearts;
        currentHearts = RunManager.instance.currentHearts;
        soulHearts = RunManager.instance.soulHearts;

        // Find UI if not assigned
        if (heartUI == null)
            heartUI = FindFirstObjectByType<HeartUI>();

        if (heartUI != null)
        {
            heartUI.Initialize(maxHearts, soulHearts);
            heartUI.UpdateHearts(currentHearts, soulHearts);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        stats.OnStatsChanged += RecalculateHearts;
    }

    // --------------------
    // DAMAGE
    // --------------------
    public void TakeDamage(int dmg)
    {
        if (invulnerable) return;

        // Soul hearts absorb first
        if (soulHearts > 0)
        {
            soulHearts -= dmg;

            if (soulHearts < 0)
            {
                currentHearts += soulHearts; // negative overflow
                soulHearts = 0;
            }
        }
        else
        {
            currentHearts -= dmg;
        }

        currentHearts = Mathf.Max(currentHearts, 0);

        // Save to RunManager
        RunManager.instance.currentHearts = currentHearts;
        RunManager.instance.soulHearts = soulHearts;

        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);

        HitStopController.instance.Stop(0.05f);
        StartCoroutine(Invulnerability());

        if (currentHearts <= 0)
            Die();
    }

    public void AddSoulHearts(int amount)
    {
        soulHearts += amount;

        RunManager.instance.soulHearts = soulHearts;

        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
    }

    private void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
        Time.timeScale = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --------------------
    // HEALING
    // --------------------
    public void Heal(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Min(currentHearts, maxHearts);

        RunManager.instance.currentHearts = currentHearts;

        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);
    }

    // --------------------
    // STAT-DRIVEN HEART UPDATES
    // --------------------
    private void RecalculateHearts()
    {
        maxHearts = RunManager.instance.maxHearts + stats.maxHeartsModifier;

        soulHearts += stats.soulHeartsModifier;
        soulHearts = Mathf.Max(0, soulHearts);

        stats.soulHeartsModifier = 0;

        currentHearts = Mathf.Min(currentHearts, maxHearts);

        RunManager.instance.maxHearts = maxHearts;
        RunManager.instance.currentHearts = currentHearts;
        RunManager.instance.soulHearts = soulHearts;

        if (heartUI != null)
        {
            heartUI.Initialize(maxHearts, soulHearts);
            heartUI.UpdateHearts(currentHearts, soulHearts);
        }
    }

    private void FindOrCreateHeartUI()
    {
        if (heartUI != null)
            return;

        heartUI = FindFirstObjectByType<HeartUI>();

        if (heartUI == null)
        {
            GameObject prefab = Resources.Load<GameObject>("HeartUI");
            GameObject ui = Instantiate(prefab);
            heartUI = ui.GetComponent<HeartUI>();
        }
    }

}
